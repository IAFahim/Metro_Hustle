using System.Collections.Generic;
using System.Linq;
using ECS_Spline.Runtime.Datas;
using ECSUnitySplineAddon.Runtime.Datas;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace ECSSplines.Runtime
{
    public static class NativeSplineContainerBlobFactory
    {
        public const int LUT_RESOLUTION = 30;

        /// <summary>
        /// Creates a BlobAssetReference for a NativeSplineContainerBlob from a SplineContainer.
        /// </summary>
        public static BlobAssetReference<NativeSplineContainerBlob> CreateBlob(
            SplineContainer container,
            float4x4 bakingTransform,
            bool cacheUpVectors)
        {
            if (container == null || container.Splines == null || container.Splines.Count == 0)
            {
                Debug.LogWarning("Cannot bake empty SplineContainer.", container);
                return BlobAssetReference<NativeSplineContainerBlob>.Null;
            }

            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<NativeSplineContainerBlob>();

            int numSplines = container.Splines.Count;
            var validSplines = container.Splines.Where(s => s != null && s.Count >= 2).ToList();
            int numValidSplines = validSplines.Count;

            if (numValidSplines == 0)
            {
                Debug.LogWarning("SplineContainer has no splines with 2 or more knots. Baking an empty blob.", container);
                builder.Allocate(ref root.SplineMetadatas, 0);
                builder.Allocate(ref root.AllKnots, 0);
                builder.Allocate(ref root.AllCurves, 0);
                builder.Allocate(ref root.DistanceLUT, 0);
                builder.Allocate(ref root.UpVectorLUT, 0);
                builder.Allocate(ref root.LinkGroupMetadatas, 0);
                builder.Allocate(ref root.AllLinks, 0);
                 root.DistanceLutResolution = LUT_RESOLUTION;
                return builder.CreateBlobAssetReference<NativeSplineContainerBlob>(Allocator.Persistent);
            }


            int totalKnots = 0;
            int totalCurves = 0;
            foreach (var spline in validSplines)
            {
                totalKnots += spline.Count;
                totalCurves += spline.Closed ? spline.Count : spline.Count - 1;
            }

            BlobBuilderArray<SplineMetadataInBlob> splineMetaBuilder = builder.Allocate(ref root.SplineMetadatas, numValidSplines);
            BlobBuilderArray<BlobBezierKnot> knotsBuilder = builder.Allocate(ref root.AllKnots, totalKnots);
            BlobBuilderArray<BlobBezierCurve> curvesBuilder = builder.Allocate(ref root.AllCurves, totalCurves);
            BlobBuilderArray<DistanceToInterpolation> distLutBuilder = builder.Allocate(ref root.DistanceLUT, totalCurves * LUT_RESOLUTION);
            BlobBuilderArray<float3> upVecLutBuilder = cacheUpVectors
                ? builder.Allocate(ref root.UpVectorLUT, totalCurves * LUT_RESOLUTION)
                : builder.Allocate(ref root.UpVectorLUT, 0);

            root.DistanceLutResolution = LUT_RESOLUTION;

            int currentKnotOffset = 0;
            int currentCurveOffset = 0;
            var tempDistLut = new NativeArray<DistanceToInterpolation>(LUT_RESOLUTION, Allocator.Temp);
            var tempUpLut = cacheUpVectors ? new NativeArray<float3>(LUT_RESOLUTION, Allocator.Temp) : default;

            for (int i = 0; i < numValidSplines; ++i)
            {
                Spline spline = validSplines[i];
                int knotCount = spline.Count;
                int curveCount = spline.Closed ? knotCount : knotCount - 1;

                using var nativeSpline = new NativeSpline(spline, bakingTransform, cacheUpVectors, Allocator.Temp);


                ref var meta = ref splineMetaBuilder[i];
                meta.KnotStartIndex = currentKnotOffset;
                meta.KnotCount = knotCount;
                meta.CurveStartIndex = currentCurveOffset;
                meta.CurveCount = curveCount;
                meta.DistLutStartIndex = currentCurveOffset * LUT_RESOLUTION;
                meta.UpVectorLutStartIndex = cacheUpVectors ? currentCurveOffset * LUT_RESOLUTION : -1;
                meta.Length = nativeSpline.GetLength();
                meta.Closed = spline.Closed;

                for (int k = 0; k < knotCount; ++k)
                {
                    BezierKnot sourceKnot = nativeSpline.Knots[k];
                     knotsBuilder[currentKnotOffset + k] = new BlobBezierKnot
                     {
                          Position = sourceKnot.Position,
                          TangentIn = sourceKnot.TangentIn,
                          TangentOut = sourceKnot.TangentOut,
                          Rotation = sourceKnot.Rotation
                     };
                }

                for (int c = 0; c < curveCount; ++c)
                {
                    BezierCurve curve = nativeSpline.Curves[c];
                    curvesBuilder[currentCurveOffset + c] = new BlobBezierCurve { P0 = curve.P0, P1 = curve.P1, P2 = curve.P2, P3 = curve.P3 };


                    CurveUtility.CalculateCurveLengths(curve, tempDistLut);
                    for (int j = 0; j < LUT_RESOLUTION; ++j)
                    {
                        distLutBuilder[meta.DistLutStartIndex + c * LUT_RESOLUTION + j] = tempDistLut[j];
                    }

                    if (cacheUpVectors)
                    {
                         BezierKnot knotStart = nativeSpline.Knots[c];
                         BezierKnot knotEnd = nativeSpline.Knots[nativeSpline.Closed ? (c + 1) % knotCount : c + 1];
                         float3 startUp = math.rotate(knotStart.Rotation, math.up());
                         float3 endUp = math.rotate(knotEnd.Rotation, math.up());

                         CurveUtilityInternal.EvaluateUpVectors(curve, startUp, endUp, tempUpLut);
                        for (int j = 0; j < LUT_RESOLUTION; ++j)
                        {
                            upVecLutBuilder[meta.UpVectorLutStartIndex + c * LUT_RESOLUTION + j] = tempUpLut[j];
                        }
                    }
                }

                currentKnotOffset += knotCount;
                currentCurveOffset += curveCount;
            }

            tempDistLut.Dispose();
            if (cacheUpVectors) tempUpLut.Dispose();

            ProcessKnotLinks(builder, ref root, container, validSplines);


            var blobRef = builder.CreateBlobAssetReference<NativeSplineContainerBlob>(Allocator.Persistent);
            return blobRef;
        }


        private static void ProcessKnotLinks(BlobBuilder builder, ref NativeSplineContainerBlob root, SplineContainer container, List<Spline> validSplines)
        {
            var originalToValidIndexMap = new Dictionary<int, int>();
             for(int i = 0; i < container.Splines.Count; ++i)
             {
                 int validIndex = validSplines.IndexOf(container.Splines[i]);
                 if(validIndex >= 0) {
                      originalToValidIndexMap[i] = validIndex;
                 }
             }


            var linkGroups = new List<List<BlobSplineKnotIndex>>();
            var processedOriginalIndices = new HashSet<SplineKnotIndex>();

            for (int originalSplineIdx = 0; originalSplineIdx < container.Splines.Count; ++originalSplineIdx)
            {
                if (!originalToValidIndexMap.ContainsKey(originalSplineIdx)) continue;

                Spline spline = container.Splines[originalSplineIdx];
                for (int knotIdx = 0; knotIdx < spline.Count; ++knotIdx)
                {
                    var currentOriginalIndex = new SplineKnotIndex(originalSplineIdx, knotIdx);

                    if (processedOriginalIndices.Contains(currentOriginalIndex)) continue;

                    if (container.KnotLinkCollection.TryGetKnotLinks(currentOriginalIndex, out var linkedKnotsRO))
                    {
                        var currentGroup = new List<BlobSplineKnotIndex>();
                        foreach(var originalLink in linkedKnotsRO)
                        {
                             if(originalToValidIndexMap.TryGetValue(originalLink.Spline, out int validSplineIdx))
                             {
                                 if (originalLink.Knot >= 0 && originalLink.Knot < validSplines[validSplineIdx].Count)
                                 {
                                     currentGroup.Add(new BlobSplineKnotIndex { SplineIndex = validSplineIdx, KnotIndex = originalLink.Knot });
                                     processedOriginalIndices.Add(originalLink);
                                 }
                             }
                        }


                        if (currentGroup.Count > 1)
                        {
                            linkGroups.Add(currentGroup);
                        }
                         else if (currentGroup.Count == 1)
                         {
                             processedOriginalIndices.Add(currentOriginalIndex);
                         }
                    }
                    else
                    {
                        processedOriginalIndices.Add(currentOriginalIndex);
                    }
                }
            }

            int totalLinks = linkGroups.Sum(group => group.Count);
            BlobBuilderArray<LinkGroupMetadataInBlob> linkMetaBuilder = builder.Allocate(ref root.LinkGroupMetadatas, linkGroups.Count);
            BlobBuilderArray<BlobSplineKnotIndex> allLinksBuilder = builder.Allocate(ref root.AllLinks, totalLinks);

            int currentLinkOffset = 0;
            for (int g = 0; g < linkGroups.Count; ++g)
            {
                var group = linkGroups[g];
                linkMetaBuilder[g] = new LinkGroupMetadataInBlob
                {
                    LinkStartIndex = currentLinkOffset,
                    LinkCount = group.Count
                };

                for (int l = 0; l < group.Count; ++l)
                {
                    allLinksBuilder[currentLinkOffset + l] = group[l];
                }
                currentLinkOffset += group.Count;
            }
        }
    }
}