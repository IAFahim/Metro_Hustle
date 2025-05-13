using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace ECSUnitySplineAddon.Runtime.Datas
{
    public static class NativeSplineBlobFactory
    {
        /// <summary>
        /// Creates a BlobAssetReference for a NativeSplineBlob from a managed Spline.
        /// </summary>
        /// <param name="spline">The managed Spline object.</param>
        /// <param name="bakingTransform">Transform to bake the spline into (e.g., localToWorld or identity for local space).</param>
        /// <param name="allocator">The allocator for the Blob Builder.</param>
        /// <returns>A BlobAssetReference to the created spline data.</returns>
        public static BlobAssetReference<NativeSplineBlob> CreateBlob(
            Spline spline,
            float4x4 bakingTransform,
            int LUT_RESOLUTION,
            Allocator allocator = Allocator.Temp)
        {
            using var nativeSpline = new NativeSpline(spline, bakingTransform, Allocator.Temp);
            return CreateBlob(nativeSpline, LUT_RESOLUTION, allocator);
        }

        /// <summary>
        /// Creates a BlobAssetReference for a NativeSplineBlob from an existing NativeSpline.
        /// </summary>
        /// <param name="nativeSpline">The NativeSpline data source. Assumes transform is already applied.</param>
        /// <param name="allocator">The allocator for the Blob Builder.</param>
        /// <returns>A BlobAssetReference to the created spline data.</returns>
        public static BlobAssetReference<NativeSplineBlob> CreateBlob(
            NativeSpline nativeSpline, int LUT_RESOLUTION, Allocator allocator = Allocator.Temp)
        {
            if (nativeSpline.Count < 2)
            {
                Debug.LogWarning("Attempting to bake a spline with less than 2 knots. Creating a minimal blob.");
            }

            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<NativeSplineBlob>();

            int knotCount = nativeSpline.Count;
            int curveCount = knotCount > 0 ? (nativeSpline.Closed ? knotCount : knotCount - 1) : 0;

            BlobBuilderArray<BezierCurve> curvesBuilder = builder.Allocate(ref root.Curves, curveCount);
            BlobBuilderArray<DistanceToInterpolation> distLutBuilder =
                builder.Allocate(ref root.DistanceLUT, curveCount * LUT_RESOLUTION);
            BlobBuilderArray<float3> upVecLutBuilder =
                builder.Allocate(ref root.UpVectorLUT, curveCount * LUT_RESOLUTION);

            if (knotCount > 0)
            {
                float totalLength = 0f;
                var tempDistLut = new NativeArray<DistanceToInterpolation>(LUT_RESOLUTION, Allocator.Temp);
                var tempUpLut = new NativeArray<float3>(LUT_RESOLUTION, Allocator.Temp);

                for (int i = 0; i < curveCount; ++i)
                {
                    BezierCurve curve = nativeSpline.Curves[i];
                    curvesBuilder[i] = curve;

                    CurveUtility.CalculateCurveLengths(curve, tempDistLut);
                    float currentCurveLength = tempDistLut[LUT_RESOLUTION - 1].Distance;
                    totalLength += currentCurveLength;

                    for (int j = 0; j < LUT_RESOLUTION; ++j)
                    {
                        distLutBuilder[i * LUT_RESOLUTION + j] = tempDistLut[j];
                    }

                    BezierKnot knotStart = nativeSpline.Knots[i];
                    BezierKnot knotEnd = nativeSpline.Knots[nativeSpline.Closed ? (i + 1) % knotCount : i + 1];
                    float3 startUp = math.rotate(knotStart.Rotation, math.up());
                    float3 endUp = math.rotate(knotEnd.Rotation, math.up());

                    CurveUtilityInternal.EvaluateUpVectors(curve, startUp, endUp, tempUpLut);

                    for (int j = 0; j < LUT_RESOLUTION; ++j)
                    {
                        upVecLutBuilder[i * LUT_RESOLUTION + j] = tempUpLut[j];
                    }
                }

                root.Length = totalLength;
                root.Closed = nativeSpline.Closed;

                tempDistLut.Dispose();
            }
            else
            {
                root.Length = 0f;
                root.Closed = false;
            }

            root.LUT_RESOLUTION = LUT_RESOLUTION;

            var blobRef = builder.CreateBlobAssetReference<NativeSplineBlob>(Allocator.Persistent);
            return blobRef;
        }
    }
}