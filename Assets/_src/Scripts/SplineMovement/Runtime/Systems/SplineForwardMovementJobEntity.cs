// using _src.Scripts.Speeds.Runtime.Datas;
// using _src.Scripts.SplineMovement.Runtime.Datas;
// using ECSUnitySplineAddon.Runtime.Datas;
// using Unity.Burst; // Assuming other components are here
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine.Splines; // For CurveUtility
//
// namespace _src.Scripts.SplineMovement.Runtime.Systems
// {
//     [BurstCompile] // Add BurstCompile for performance
//     public partial struct SplineForwardMovementJobEntity : IJobEntity
//     {
//         [ReadOnly] public float TimeDelta;
//         [ReadOnly] public LocalTransform SplineLocalTransform; // Transform of the spline object itself
//         [ReadOnly] public BlobAssetReference<NativeSplineBlob> NativeSplineBlob;
//
//         private void Execute(ref LocalTransform localTransform, ref SplineLinkComponentData splineLinkComponentData,
//             in SpeedComponentData speedComponentData)
//         {
//             // --- Handle Progress & Knot Index ---
//             if (splineLinkComponentData.Progress >= 1)
//             {
//                 splineLinkComponentData.Progress = 0; // Reset progress for the new knot
//                 splineLinkComponentData.CurveIndex++;
//                 // Optional: Add logic here if KnotIndex goes beyond the number of curves
//                 if (splineLinkComponentData.CurveIndex >= NativeSplineBlob.Value.Curves.Length)
//                 {
//                     // Stop, loop, destroy, etc. Example: Loop back to start
//                     splineLinkComponentData.CurveIndex = 0;
//                 }
//             }
//
//             // --- Calculate Movement Along Spline ---
//             var displacement = speedComponentData.GetCurrentSpeed() * TimeDelta;
//             // TODO: Need to handle displacement potentially crossing knot boundaries within one frame.
//             // For simplicity now, assume displacement is smaller than remaining distance in the current knot.
//             splineLinkComponentData.Progress += displacement; // Update progress *before* evaluating
//             splineLinkComponentData.Progress = math.min(splineLinkComponentData.Progress, 1.0f); // Clamp progress
//
//             var progress = splineLinkComponentData.Progress;
//             var currentKnotIndex = splineLinkComponentData.CurveIndex;
//
//             var curve = NativeSplineBlob.Value.Curves[currentKnotIndex];
//
//             // --- Evaluate Spline in its Local Space ---
//             float3 localSplinePos = CurveUtility.EvaluatePosition(curve, progress);
//             float3 localSplineTangent = CurveUtility.EvaluateTangent(curve, progress);
//             // Optional: Evaluate Up vector if spline has twist/roll
//             // float3 localSplineUp = CurveUtility.EvaluateUpVector(curve, progress, math.up());
//
//             // --- Transform to World Space ---
//             // Assumes SplineLocalTransform holds the world transform of the spline object
//             float4x4 splineWorldMatrix = SplineLocalTransform.ToMatrix();
//             float3 worldSplinePos = math.transform(splineWorldMatrix, localSplinePos);
//             // Only apply rotation to direction vectors (tangent), normalize for safety
//             float3 worldSplineTangent =
//                 math.normalize(math.rotate(SplineLocalTransform.Rotation, localSplineTangent));
//
//             // Define world up direction
//             float3 worldUp = math.up(); // Typically (0, 1, 0)
//
//             // --- Calculate Perpendicular Offset Direction (World Right) ---
//             // Use cross product: Up x Forward = Right (adjust if using left-handed convention or different Up)
//             float3 worldSplineRight = math.normalize(math.cross(worldUp, worldSplineTangent));
//
//             // If tangent is parallel to Up, cross product is zero. Handle this edge case.
//             // A common fallback is to use the world's X or Z axis if the primary 'right' fails.
//             if (math.lengthsq(worldSplineRight) < 0.0001f)
//             {
//                 // Fallback: Use world right if tangent is vertical
//                 worldSplineRight =
//                     math.normalize(math.cross(worldSplineTangent,
//                         math.forward())); // Or use math.right() directly if tangent isn't axis aligned
//                 if (math.lengthsq(worldSplineRight) < 0.0001f) // If still parallel (e.g., tangent == forward)
//                 {
//                     worldSplineRight = math.right(); // Absolute fallback
//                 }
//             }
//
//
//             // --- Calculate Final Position with Offset ---
//             // splineLineComponentData.Value determines the distance and direction (+/-)
//             float desiredOffset = splineLinkComponentData.LineNumber; // Use the value from the component
//             float3 offsetVector = worldSplineRight * desiredOffset;
//             localTransform.Position = worldSplinePos + offsetVector;
//
//             // --- Calculate Orientation (Face along Tangent) ---
//             // Use LookRotationSafe to handle edge cases where tangent and up are aligned
//             localTransform.Rotation = quaternion.LookRotationSafe(worldSplineTangent, worldUp);
//         }
//     }
// }