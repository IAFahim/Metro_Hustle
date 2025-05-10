using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace ECSUnitySplineAddon.Runtime.Datas
{
    /// <summary>
    /// Exposing CurveUtility internal Methods.
    /// WARNING: This code is adapted from internal Unity Splines package code.
    /// It might break if the underlying package changes significantly.
    /// </summary>
    
    internal static class CurveUtilityInternal
    {
        private struct FrenetFrame
        {
            public float3 origin;
            public float3 tangent;
            public float3 normal;
            public float3 binormal;
        }

        private const int k_NormalsPerCurve = 16;
        private const float k_Epsilon = 0.0001f;

        
        private static bool Approximately(float a, float b)
        {
            return math.abs(b - a) < math.max(0.000001f * math.max(math.abs(a), math.abs(b)), k_Epsilon * 8);
        }

        internal static float3 GetExplicitLinearTangent(float3 point, float3 to)
        {
            return (to - point) / 3.0f;
        }

        /// <summary>
        /// Evaluates multiple up-vectors along a curve using Rotation Minimizing Frames (RMF).
        /// Populates the pre-allocated upVectors array.
        /// </summary>
        
        internal static void EvaluateUpVectors(BezierCurve curve, float3 startUp, float3 endUp, NativeArray<float3> upVectors)
        {
            int resolution = upVectors.Length;
            if (resolution < 2) return;

            upVectors[0] = startUp;
            upVectors[resolution - 1] = endUp;

            for (int i = 1; i < resolution - 1; i++)
            {
                var curveT = (float)i / (resolution - 1);
                upVectors[i] = EvaluateUpVector(curve, curveT, startUp, endUp);
            }
        }

        /// <summary>
        /// Evaluates a single up-vector at a specific point 't' on the curve using RMF.
        /// </summary>
        
        internal static float3 EvaluateUpVector(BezierCurve curve, float t, float3 startUp, float3 endUp, bool fixEndUpMismatch = true)
        {
            var linearTangentLen = math.length(GetExplicitLinearTangent(curve.P0, curve.P3));
            if(linearTangentLen == 0)
            {
                if(math.lengthsq(curve.P1 - curve.P0) > k_Epsilon * k_Epsilon)
                    linearTangentLen = math.length(curve.P1 - curve.P0);
                else if (math.lengthsq(curve.P2 - curve.P3) > k_Epsilon * k_Epsilon)
                    linearTangentLen = math.length(curve.P2 - curve.P3);
                else
                    return startUp;
            }

            var linearTangentOut = math.normalize(curve.P3 - curve.P0) * linearTangentLen;
             if(math.all(math.isnan(linearTangentOut))) linearTangentOut = new float3(0,0,1) * linearTangentLen;

             var tangentP0P1 = curve.P1 - curve.P0;
            var tangentP3P2 = curve.P3 - curve.P2;

            if (Approximately(math.lengthsq(tangentP0P1), 0f))
                curve.P1 = curve.P0 + linearTangentOut;
            if (Approximately(math.lengthsq(tangentP3P2), 0f))
                curve.P2 = curve.P3 - linearTangentOut;


            var normalBuffer = new NativeArray<float3>(k_NormalsPerCurve, Allocator.Temp);

            FrenetFrame frame;
            frame.origin = curve.P0;
            frame.tangent = curve.P1 - curve.P0;
            frame.normal = startUp;
            float3 crossTN = math.cross(frame.tangent, frame.normal);
            if (math.lengthsq(crossTN) < k_Epsilon * k_Epsilon) {
                float3 arbitraryVec = math.abs(math.dot(frame.tangent, new float3(0,1,0))) < 0.99f ? new float3(0,1,0) : new float3(1,0,0);
                 frame.binormal = math.normalize(math.cross(frame.tangent, arbitraryVec));
                 frame.normal = math.normalize(math.cross(frame.binormal, frame.tangent));
            } else {
                 frame.binormal = math.normalize(crossTN);
            }

            if (math.any(math.isnan(frame.binormal)))
             {
                 normalBuffer.Dispose();
                 return startUp;
             }


            normalBuffer[0] = frame.normal;

            float stepSize = 1f / (k_NormalsPerCurve - 1);
            float currentCurveT = stepSize;
            float prevCurveT = 0f;
            float3 evaluatedUpVector = float3.zero;
            bool evaluated = false;

            FrenetFrame prevFrame;
            for (int i = 1; i < k_NormalsPerCurve; ++i)
            {
                prevFrame = frame;
                frame = GetNextRotationMinimizingFrame(curve, prevFrame, currentCurveT);
                normalBuffer[i] = frame.normal;

                if (!evaluated && prevCurveT <= t && currentCurveT >= t)
                {
                    float lerpT = (t - prevCurveT) / stepSize;
                    evaluatedUpVector = Vector3.Slerp(prevFrame.normal, frame.normal, lerpT);
                    evaluated = true;
                    if(!fixEndUpMismatch) break;
                }

                prevCurveT = currentCurveT;
                currentCurveT += stepSize;
            }

            if (!evaluated && Approximately(t, 1f)) {
                 evaluatedUpVector = endUp;
                 evaluated = true;
             } else if (!evaluated) {
                evaluatedUpVector = startUp;
             }


            if (!fixEndUpMismatch)
            {
                normalBuffer.Dispose();
                return evaluatedUpVector;
            }

            float3 lastFrameNormal = normalBuffer[k_NormalsPerCurve - 1];
            float angleBetweenNormals = math.acos(math.clamp(math.dot(lastFrameNormal, endUp), -1f, 1f));

            if (Approximately(angleBetweenNormals, 0f))
            {
                 normalBuffer.Dispose();
                 return evaluatedUpVector;
            }

            float3 lastNormalTangent = math.normalize(CurveUtility.EvaluateTangent(curve, 1f));
            if(math.lengthsq(lastNormalTangent) < k_Epsilon * k_Epsilon)
                lastNormalTangent = math.normalize(curve.P3 - curve.P2);

            quaternion positiveRotation = quaternion.AxisAngle(lastNormalTangent, angleBetweenNormals);
            quaternion negativeRotation = quaternion.AxisAngle(lastNormalTangent, -angleBetweenNormals);
            float positiveRotationResult = math.acos(math.clamp(math.dot(math.rotate(positiveRotation, lastFrameNormal), endUp), -1f, 1f));
            float negativeRotationResult = math.acos(math.clamp(math.dot(math.rotate(negativeRotation, lastFrameNormal), endUp), -1f, 1f));

            if (!(positiveRotationResult < negativeRotationResult)) angleBetweenNormals = -angleBetweenNormals;


            currentCurveT = stepSize;
            prevCurveT = 0f;
            evaluated = false;

            for (int i = 1; i < normalBuffer.Length; i++)
            {
                float3 normal = normalBuffer[i];
                float adjustmentAngle = math.lerp(0f, angleBetweenNormals, currentCurveT);
                float3 tangent = math.normalize(CurveUtility.EvaluateTangent(curve, currentCurveT));
                 if (math.lengthsq(tangent) < k_Epsilon * k_Epsilon)
                     tangent = lastNormalTangent;

                 float3 adjustedNormal = math.rotate(quaternion.AxisAngle(tangent, adjustmentAngle), normal);

                normalBuffer[i] = adjustedNormal;

                if (!evaluated && prevCurveT <= t && currentCurveT >= t)
                {
                    float lerpT = (t - prevCurveT) / stepSize;
                    evaluatedUpVector = Vector3.Slerp(normalBuffer[i - 1], normalBuffer[i], lerpT);
                    evaluated = true;
                    break;
                }

                prevCurveT = currentCurveT;
                currentCurveT += stepSize;
            }

            normalBuffer.Dispose();

            if (!evaluated || Approximately(t, 1f))
                 return endUp;

            return evaluatedUpVector;
        }

        
        private static FrenetFrame GetNextRotationMinimizingFrame(BezierCurve curve, FrenetFrame previousRMFrame, float nextRMFrameT)
        {
            FrenetFrame nextRMFrame;
            nextRMFrame.origin = CurveUtility.EvaluatePosition(curve, nextRMFrameT);
            nextRMFrame.tangent = CurveUtility.EvaluateTangent(curve, nextRMFrameT);

            float3 toCurrentFrame = nextRMFrame.origin - previousRMFrame.origin;
            float c1 = math.dot(toCurrentFrame, toCurrentFrame);
             if (c1 < k_Epsilon * k_Epsilon)
             {
                 return previousRMFrame;
             }

             float3 riL = previousRMFrame.binormal - toCurrentFrame * 2f / c1 * math.dot(toCurrentFrame, previousRMFrame.binormal);
            float3 tiL = previousRMFrame.tangent - toCurrentFrame * 2f / c1 * math.dot(toCurrentFrame, previousRMFrame.tangent);

            float3 v2 = nextRMFrame.tangent - tiL;
            float c2 = math.dot(v2, v2);
             if (c2 < k_Epsilon * k_Epsilon)
             {
                 nextRMFrame.binormal = math.normalize(riL);
             }
             else
             {
                  nextRMFrame.binormal = math.normalize(riL - v2 * 2f / c2 * math.dot(v2, riL));
             }

             nextRMFrame.normal = math.normalize(math.cross(nextRMFrame.binormal, nextRMFrame.tangent));

             if (math.any(math.isnan(nextRMFrame.binormal)) || math.any(math.isnan(nextRMFrame.normal))) {
                 return previousRMFrame;
            }

            return nextRMFrame;
        }
    }
}