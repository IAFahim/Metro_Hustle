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
    /// <summary>
    /// Unmanaged, immutable representation of a single Bezier Knot for use in Blobs.
    /// Matches UnityEngine.Splines.BezierKnot layout.
    /// </summary>
    public struct BlobBezierKnot
    {
        public float3 Position;
        public float3 TangentIn;
        public float3 TangentOut;
        public quaternion Rotation;
    }

    /// <summary>
    /// Unmanaged, immutable representation of a single Bezier Curve for use in Blobs.
    /// Matches UnityEngine.Splines.BezierCurve layout.
    /// </summary>
    public struct BlobBezierCurve
    {
        public float3 P0;
        public float3 P1;
        public float3 P2;
        public float3 P3;
    }

    /// <summary>
    /// Metadata for a single spline stored within the NativeSplineContainerBlob.
    /// </summary>
    public struct SplineMetadataInBlob
    {
        public int KnotStartIndex;
        public int KnotCount;
        public int CurveStartIndex;
        public int CurveCount;
        public int DistLutStartIndex;
        public int UpVectorLutStartIndex;
        public float Length;
        public bool Closed;
    }

    /// <summary>
    /// Unmanaged representation of SplineKnotIndex (Spline + Knot index pair) for use in Blobs.
    /// Indices refer to the spline's position within the *blob* and the knot's position within *that spline's knots*.
    /// </summary>
    public struct BlobSplineKnotIndex
    {
        public int SplineIndex;
        public int KnotIndex;
    }

    /// <summary>
    /// Metadata for a group of linked knots stored within the NativeSplineContainerBlob.
    /// </summary>
    public struct LinkGroupMetadataInBlob
    {
        public int LinkStartIndex;
        public int LinkCount;
    }

    /// <summary>
    /// Unmanaged, immutable representation of an entire SplineContainer,
    /// including all its splines, lookup tables, and knot links, suitable for ECS Blob Assets.
    /// </summary>

    /// <summary>
    /// Unmanaged, immutable representation of an entire SplineContainer,
    /// including all its splines, lookup tables, and knot links, suitable for ECS Blob Assets.
    /// </summary>
    [BurstCompile]
    public struct NativeSplineContainerBlob
    {
        public BlobArray<SplineMetadataInBlob> SplineMetadatas;

        public BlobArray<BlobBezierKnot> AllKnots;
        public BlobArray<BlobBezierCurve> AllCurves;
        public BlobArray<DistanceToInterpolation> DistanceLUT;
        public BlobArray<float3> UpVectorLUT;

        public BlobArray<LinkGroupMetadataInBlob> LinkGroupMetadatas;
        public BlobArray<BlobSplineKnotIndex> AllLinks;

        public int DistanceLutResolution;

        /// <summary>Gets the number of splines stored in this blob.</summary>
        public int SplineCount => SplineMetadatas.Length;

        /// <summary>Gets the metadata for a specific spline within the blob.</summary>
        public ref readonly SplineMetadataInBlob GetSplineMetadata(int splineIndex)
        {
            return ref SplineMetadatas[splineIndex];
        }

        /// <summary>Gets a specific knot belonging to a specific spline.</summary>
        public BlobBezierKnot GetKnot(int splineIndex, int knotIndexInSpline)
        {
            ref readonly var meta = ref GetSplineMetadata(splineIndex);
            if (knotIndexInSpline < 0 || knotIndexInSpline >= meta.KnotCount) return default;
            return AllKnots[meta.KnotStartIndex + knotIndexInSpline];
        }

        /// <summary>Gets a specific curve belonging to a specific spline.</summary>
        public BlobBezierCurve GetCurve(int splineIndex, int curveIndexInSpline)
        {
            ref readonly var meta = ref GetSplineMetadata(splineIndex);
            if (curveIndexInSpline < 0 || curveIndexInSpline >= meta.CurveCount) return default;
            return AllCurves[meta.CurveStartIndex + curveIndexInSpline];
        }

        /// <summary>Gets the length of a specific curve within a specific spline.</summary>
        public float GetCurveLength(int splineIndex, int curveIndexInSpline)
        {
            ref readonly var meta = ref GetSplineMetadata(splineIndex);
            if (curveIndexInSpline < 0 || curveIndexInSpline >= meta.CurveCount || DistanceLutResolution <= 0)
                return 0f;

            int lutIndex = meta.DistLutStartIndex + curveIndexInSpline * DistanceLutResolution + DistanceLutResolution - 1;
            if (lutIndex >= DistanceLUT.Length) return 0f;
            return DistanceLUT[lutIndex].Distance;
        }

        /// <summary>
        /// Converts a normalized interpolation value (0-1) for a *specific spline* into the curve index
        /// within that spline and a normalized interpolation value (0-1) along that curve.
        /// </summary>
        public int SplineToCurveT(int splineIndex, float splineT, out float curveT)
        {
            ref readonly var meta = ref GetSplineMetadata(splineIndex);
            if (meta.KnotCount <= 1)
            {
                curveT = 0f;
                return 0;
            }

            splineT = math.clamp(splineT, 0f, 1f);
            float targetDistance = splineT * meta.Length;
            float accumulatedDistance = 0f;

            for (int curveIdxInSpline = 0; curveIdxInSpline < meta.CurveCount; ++curveIdxInSpline)
            {
                float currentCurveLength = GetCurveLength(splineIndex, curveIdxInSpline);
                if (targetDistance <= accumulatedDistance + currentCurveLength + 0.0001f)
                {
                    float distanceIntoCurve = targetDistance - accumulatedDistance;
                    curveT = GetCurveInterpolationFromDistance(splineIndex, curveIdxInSpline, distanceIntoCurve);
                    return curveIdxInSpline;
                }
                accumulatedDistance += currentCurveLength;
            }

            curveT = 1f;
            return meta.CurveCount - 1;
        }

        /// <summary>Gets curve-local T from distance using the LUT for a specific curve.</summary>
        private float GetCurveInterpolationFromDistance(int splineIndex, int curveIndexInSpline, float distanceInCurve)
        {
            if (distanceInCurve <= 0f) return 0f;

            ref readonly var meta = ref GetSplineMetadata(splineIndex);
            int lutStartIndex = meta.DistLutStartIndex + curveIndexInSpline * DistanceLutResolution;
            int lutEndIndex = lutStartIndex + DistanceLutResolution - 1;

             if (lutEndIndex >= DistanceLUT.Length) return 1f;
             if (distanceInCurve >= DistanceLUT[lutEndIndex].Distance) return 1f;

             for (int i = 0; i < DistanceLutResolution - 1; ++i)
            {
                int currentLutIndex = lutStartIndex + i;
                ref readonly var prev = ref DistanceLUT[currentLutIndex];
                ref readonly var next = ref DistanceLUT[currentLutIndex + 1];

                if (distanceInCurve < next.Distance)
                {
                    float segmentLength = next.Distance - prev.Distance;
                    if (segmentLength <= 0.00001f) return prev.T;
                    float lerpFactor = (distanceInCurve - prev.Distance) / segmentLength;
                    return math.lerp(prev.T, next.T, lerpFactor);
                }
            }
            return 1f;
        }

        /// <summary>Evaluates position on a specific spline at normalized time t.</summary>
        public float3 EvaluatePosition(int splineIndex, float splineT)
        {
            ref readonly var meta = ref GetSplineMetadata(splineIndex);
            if (meta.KnotCount == 0) return float3.zero;
            if (meta.KnotCount == 1) return GetKnot(splineIndex, 0).Position;

            int curveIdx = SplineToCurveT(splineIndex, splineT, out float curveT);
            BlobBezierCurve curve = GetCurve(splineIndex, curveIdx);
            return EvaluatePositionInternal(curve, curveT);
        }

         /// <summary>Evaluates tangent on a specific spline at normalized time t.</summary>
        public float3 EvaluateTangent(int splineIndex, float splineT)
        {
            ref readonly var meta = ref GetSplineMetadata(splineIndex);
             if (meta.KnotCount < 2) return new float3(0, 0, 1);

            int curveIdx = SplineToCurveT(splineIndex, splineT, out float curveT);
            BlobBezierCurve curve = GetCurve(splineIndex, curveIdx);
            return EvaluateTangentInternal(curve, curveT);
        }

        /// <summary>Evaluates up vector on a specific spline at normalized time t.</summary>
        public float3 EvaluateUpVector(int splineIndex, float splineT)
        {
             ref readonly var meta = ref GetSplineMetadata(splineIndex);
             if (meta.KnotCount < 2) return new float3(0, 1, 0);

            int curveIdx = SplineToCurveT(splineIndex, splineT, out float curveT);

            if (meta.UpVectorLutStartIndex >= 0 && UpVectorLUT.Length > 0)
            {
                 int lutResolution = UpVectorLUT.Length / meta.CurveCount;
                 int lutStartIndex = meta.UpVectorLutStartIndex + curveIdx * lutResolution;

                 if(lutResolution == 0) goto CalculateDynamically;

                 float segmentT = curveT * (lutResolution - 1);
                int index0 = math.min((int)math.floor(segmentT), lutResolution - 2);
                int index1 = index0 + 1;
                float lerpFactor = segmentT - index0;

                 int globalIndex0 = lutStartIndex + index0;
                 int globalIndex1 = lutStartIndex + index1;

                 if (globalIndex0 < 0 || globalIndex1 >= UpVectorLUT.Length) goto CalculateDynamically;

                 return Vector3.Slerp(UpVectorLUT[globalIndex0], UpVectorLUT[globalIndex1], lerpFactor);
            }

            CalculateDynamically:
            BlobBezierCurve curve = GetCurve(splineIndex, curveIdx);
            BlobBezierKnot knotStart = GetKnot(splineIndex, curveIdx);
            int endKnotIndexInSpline = meta.Closed ? (curveIdx + 1) % meta.KnotCount : math.min(curveIdx + 1, meta.KnotCount - 1);
            BlobBezierKnot knotEnd = GetKnot(splineIndex, endKnotIndexInSpline);

            float3 startUp = math.rotate(knotStart.Rotation, math.up());
            float3 endUp = math.rotate(knotEnd.Rotation, math.up());

            BezierCurve managedCurve = new BezierCurve { P0=curve.P0, P1=curve.P1, P2=curve.P2, P3=curve.P3};
            return CurveUtilityInternal.EvaluateUpVector(managedCurve, curveT, startUp, endUp);
        }

         /// <summary>Evaluates position, tangent, and up vector efficiently.</summary>
        public void Evaluate(int splineIndex, float splineT, out float3 position, out float3 tangent, out float3 upVector)
        {
             ref readonly var meta = ref GetSplineMetadata(splineIndex);
              if (meta.KnotCount == 0)
             {
                 position = float3.zero; tangent = new float3(0, 0, 1); upVector = new float3(0, 1, 0); return;
             }
             if (meta.KnotCount == 1)
             {
                  BlobBezierKnot knot = GetKnot(splineIndex, 0);
                 position = knot.Position; tangent = math.rotate(knot.Rotation, new float3(0, 0, 1)); upVector = math.rotate(knot.Rotation, new float3(0, 1, 0)); return;
             }


            int curveIdx = SplineToCurveT(splineIndex, splineT, out float curveT);
            BlobBezierCurve curve = GetCurve(splineIndex, curveIdx);

            position = EvaluatePositionInternal(curve, curveT);
            tangent = EvaluateTangentInternal(curve, curveT);
            upVector = EvaluateUpVector(splineIndex, splineT);
        }

        
        private static float3 EvaluatePositionInternal(BlobBezierCurve curve, float t)
        {
            t = math.clamp(t, 0, 1);
            float mt = 1f - t;
            float mt2 = mt * mt;
            float t2 = t * t;
            return (mt2 * mt * curve.P0) + (3f * mt2 * t * curve.P1) + (3f * mt * t2 * curve.P2) + (t2 * t * curve.P3);
        }

        private static float3 EvaluateTangentInternal(BlobBezierCurve curve, float t)
        {
             t = math.clamp(t, 0, 1);
             float mt = 1.0f - t;
             return (3f * mt * mt * (curve.P1 - curve.P0)) + (6f * mt * t * (curve.P2 - curve.P1)) + (3f * t * t * (curve.P3 - curve.P2));
        }

        /// <summary>Gets the number of knot link groups stored in the blob.</summary>
         public int LinkGroupCount => LinkGroupMetadatas.Length;

         /// <summary>Gets the metadata for a specific link group.</summary>
         public ref readonly LinkGroupMetadataInBlob GetLinkGroupMetadata(int groupIndex)
         {
             return ref LinkGroupMetadatas[groupIndex];
         }

         /// <summary>Gets a specific link from the flattened link array using group metadata.</summary>
         public BlobSplineKnotIndex GetLink(int groupIndex, int linkIndexInGroup)
         {
             ref readonly var groupMeta = ref GetLinkGroupMetadata(groupIndex);
             if (linkIndexInGroup < 0 || linkIndexInGroup >= groupMeta.LinkCount) return default;
             return AllLinks[groupMeta.LinkStartIndex + linkIndexInGroup];
         }
    }
}