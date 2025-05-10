using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace ECSUnitySplineAddon.Runtime.Datas
{
    /// <summary>
    /// Unmanaged representation of a spline, stored as a Blob Asset for ECS.
    /// Contains pre-calculated curve data and lookup tables for efficient runtime evaluation.
    /// </summary>
    /// <summary>
    /// Unmanaged representation of a spline, stored as a Blob Asset for ECS.
    /// Contains pre-calculated curve data and lookup tables for efficient runtime evaluation.
    /// </summary>
    [BurstCompile]
    public struct NativeSplineBlob
    {
        public BlobArray<BezierKnot> Knots;
        public BlobArray<BezierCurve> Curves;
        public BlobArray<DistanceToInterpolation> DistanceLUT;
        public BlobArray<float3> UpVectorLUT;
        public bool Closed;
        public float Length;

        /// <summary>
        /// Gets the number of curves in the spline.
        /// </summary>
        public int CurveCount => Curves.Length;

        /// <summary>
        /// Gets the BezierCurve data for a specific curve index.
        /// </summary>
        /// <param name="curveIndex">The index of the curve.</param>
        /// <returns>The BezierCurve data.</returns>
        public BezierCurve GetCurve(int curveIndex)
        {
            if (curveIndex < 0 || curveIndex >= CurveCount)
                return default;
            return Curves[curveIndex];
        }

        /// <summary>
        /// Gets the pre-calculated length of a specific curve segment.
        /// </summary>
        /// <param name="curveIndex">The index of the curve.</param>
        /// <returns>The length of the curve.</returns>
        public float GetCurveLength(int curveIndex)
        {
            if (curveIndex < 0 || curveIndex >= CurveCount)
                return 0f;

            int lutResolution = DistanceLUT.Length / CurveCount;
            if (lutResolution == 0) return 0f;

            int lastLutIndex = curveIndex * lutResolution + lutResolution - 1;
            if (lastLutIndex >= DistanceLUT.Length) return 0f;

            return DistanceLUT[lastLutIndex].Distance;
        }

        /// <summary>
        /// Converts a normalized spline interpolation value (0-1) into a specific curve index
        /// and a normalized interpolation value (0-1) along that curve.
        /// </summary>
        /// <param name="splineT">Normalized interpolation along the entire spline (0-1).</param>
        /// <param name="curveT">Output: Normalized interpolation along the specific curve (0-1).</param>
        /// <returns>The index of the curve containing the point.</returns>
        public int SplineToCurveT(float splineT, out float curveT)
        {
            int knotCount = Knots.Length;
            if (knotCount <= 1)
            {
                curveT = 0f;
                return 0;
            }

            splineT = math.clamp(splineT, 0f, 1f);
            float targetDistance = splineT * Length;

            float accumulatedDistance = 0f;
            int curveCount = CurveCount;
            int lutResolution = DistanceLUT.Length / curveCount;

            for (int i = 0; i < curveCount; ++i)
            {
                float currentCurveLength = GetCurveLength(i);

                if (targetDistance <= accumulatedDistance + currentCurveLength + 0.0001f)
                {
                    float distanceIntoCurve = targetDistance - accumulatedDistance;

                    curveT = GetCurveInterpolationFromDistance(i, distanceIntoCurve, lutResolution);
                    return i;
                }

                accumulatedDistance += currentCurveLength;
            }

            curveT = 1f;
            return curveCount - 1;
        }

        /// <summary>
        /// Gets the curve-local normalized T value corresponding to a distance along that curve, using the LUT.
        /// </summary>
        private float GetCurveInterpolationFromDistance(int curveIndex, float distanceInCurve, int lutResolution)
        {
            if (distanceInCurve <= 0f) return 0f;

            int lutStartIndex = curveIndex * lutResolution;
            int lutEndIndex = lutStartIndex + lutResolution - 1;

            if (distanceInCurve >= DistanceLUT[lutEndIndex].Distance) return 1f;

            for (int i = 0; i < lutResolution - 1; ++i)
            {
                int currentLutIndex = lutStartIndex + i;
                ref var prev = ref DistanceLUT[currentLutIndex];
                ref var next = ref DistanceLUT[currentLutIndex + 1];

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


        /// <summary>
        /// Evaluates the position on the spline at a normalized interpolation value t.
        /// </summary>
        /// <param name="splineT">Normalized interpolation along the entire spline (0-1).</param>
        /// <returns>The position on the spline.</returns>
        public float3 EvaluatePosition(float splineT)
        {
            if (Knots.Length == 0) return float3.zero;
            if (Knots.Length == 1) return Knots[0].Position;

            int curveIndex = SplineToCurveT(splineT, out float curveT);
            return CurveUtility.EvaluatePosition(Curves[curveIndex], curveT);
        }

        /// <summary>
        /// Evaluates the tangent (direction) on the spline at a normalized interpolation value t.
        /// </summary>
        /// <param name="splineT">Normalized interpolation along the entire spline (0-1).</param>
        /// <returns>The tangent vector on the spline (not normalized).</returns>
        public float3 EvaluateTangent(float splineT)
        {
            if (Knots.Length < 2) return new float3(0, 0, 1);

            int curveIndex = SplineToCurveT(splineT, out float curveT);
            return CurveUtility.EvaluateTangent(Curves[curveIndex], curveT);
        }

        /// <summary>
        /// Evaluates the 'up' vector on the spline at a normalized interpolation value t.
        /// Uses the pre-cached LUT if available, otherwise calculates it.
        /// </summary>
        /// <param name="splineT">Normalized interpolation along the entire spline (0-1).</param>
        /// <returns>The up vector on the spline.</returns>
        public float3 EvaluateUpVector(float splineT)
        {
            if (Knots.Length < 2) return new float3(0, 1, 0);

            int curveIndex = SplineToCurveT(splineT, out float curveT);

            if (UpVectorLUT.Length > 0)
            {
                int lutResolution = UpVectorLUT.Length / CurveCount;
                int lutStartIndex = curveIndex * lutResolution;

                float segmentT = curveT * (lutResolution - 1);
                int index0 = math.min((int)math.floor(segmentT), lutResolution - 2);
                int index1 = index0 + 1;

                float lerpFactor = segmentT - index0;

                float3 up0 = UpVectorLUT[lutStartIndex + index0];
                float3 up1 = UpVectorLUT[lutStartIndex + index1];

                return Vector3.Slerp(up0, up1, lerpFactor);
            }
            else
            {
                BezierCurve curve = Curves[curveIndex];
                BezierKnot knotStart = Knots[curveIndex];
                BezierKnot knotEnd =
                    Knots[Closed ? (curveIndex + 1) % Knots.Length : math.min(curveIndex + 1, Knots.Length - 1)];

                float3 startUp = math.rotate(knotStart.Rotation, math.up());
                float3 endUp = math.rotate(knotEnd.Rotation, math.up());

                return CurveUtilityInternal.EvaluateUpVector(curve, curveT, startUp, endUp);
            }
        }

        /// <summary>
        /// Evaluates the position, tangent, and up vector on the spline at a normalized interpolation value t.
        /// More efficient than calling EvaluatePosition, EvaluateTangent, and EvaluateUpVector separately.
        /// </summary>
        /// <param name="splineT">Normalized interpolation along the entire spline (0-1).</param>
        /// <param name="position">Output: The position on the spline.</param>
        /// <param name="tangent">Output: The tangent vector on the spline (not normalized).</param>
        /// <param name="upVector">Output: The up vector on the spline.</param>
        public void Evaluate(float splineT, out float3 position, out float3 tangent, out float3 upVector)
        {
            int curveIndex = SplineToCurveT(splineT, out float curveT);
            Evaluate(curveIndex, curveT, out position, out tangent, out upVector);
        }

        [BurstCompile]
        public void Evaluate(int curveIndex, float curveT, out float3 position, out float3 tangent,
            out float3 upVector)
        {
            BezierCurve curve = Curves[curveIndex];
            position = CurveUtility.EvaluatePosition(curve, curveT);
            tangent = CurveUtility.EvaluateTangent(curve, curveT);

            if (UpVectorLUT.Length > 0)
            {
                int lutResolution = UpVectorLUT.Length / CurveCount;
                int lutStartIndex = curveIndex * lutResolution;
                float segmentT = curveT * (lutResolution - 1);
                int index0 = math.min((int)math.floor(segmentT), lutResolution - 2);
                int index1 = index0 + 1;
                float lerpFactor = segmentT - index0;
                upVector = Vector3.Slerp(UpVectorLUT[lutStartIndex + index0], UpVectorLUT[lutStartIndex + index1],
                    lerpFactor);
            }
            else
            {
                BezierKnot knotStart = Knots[curveIndex];
                BezierKnot knotEnd =
                    Knots[Closed ? (curveIndex + 1) % Knots.Length : math.min(curveIndex + 1, Knots.Length - 1)];
                float3 startUp = math.rotate(knotStart.Rotation, math.up());
                float3 endUp = math.rotate(knotEnd.Rotation, math.up());
                upVector = CurveUtilityInternal.EvaluateUpVector(curve, curveT, startUp, endUp);
            }
        }
    }
}