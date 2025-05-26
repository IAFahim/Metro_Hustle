#if ALINE
using _src.Scripts.Colliders.Colliders.Authoring;
using _src.Scripts.Colliders.Colliders.Data;
using Drawing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Editor
{
    [BurstCompile]
    public partial struct BoxColliderEditorAlineJobEntity : IJobEntity
    {
        public CommandBuilder Drawing;
        public quaternion EditorCameraRotation;

        [ReadOnly] public NativeArray<(LocalToWorld targetLtw, TargetBody targetBody)>.ReadOnly TargetBodyLtwList;

        // Color management
        private static readonly Color BaseBoxColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
        private static readonly Color DefaultTargetColor = new Color(0.5f, 0.7f, 1f);
        private static readonly Color TipEnteredColor = new Color(1f, 0.92f, 0.016f);
        private static readonly Color InsideColor = Color.red;
        private static readonly Color InsideLineColor = Color.magenta;
        private static readonly Color OnTopColor = new Color(0.8f, 0.4f, 1f);
        private static readonly Color StandableZoneColor = new Color(0.5f, 0.7f, 1f, 0.7f);
        private static readonly Color InactivePlaneColor = new Color(0.6f, 0.6f, 0.6f, 0.3f);
        private static readonly Color HeightLabelColor = Color.white;

        // Target identification colors (cycling through different hues)
        private static readonly Color[] TargetIdentificationColors =
        {
            new(1f, 0.3f, 0.3f), // Red
            new(0.3f, 1f, 0.3f), // Green  
            new(0.3f, 0.3f, 1f), // Blue
            new(1f, 1f, 0.3f), // Yellow
            new(1f, 0.3f, 1f), // Magenta
            new(0.3f, 1f, 1f), // Cyan
            new(1f, 0.6f, 0.3f), // Orange
            new(0.6f, 0.3f, 1f), // Purple
        };

        private const float LabelSize = 0.1f;
        private const float TextVerticalOffset = 0.1f;

        private static quaternion GetPlaneRotationForAline(float3 planeNormalY, float3 planeLocalX)
        {
            float3 planeLocalZ = math.normalize(math.cross(planeLocalX, planeNormalY));
            planeLocalX = math.normalize(math.cross(planeNormalY, planeLocalZ));
            return new quaternion(new float3x3(planeLocalX, planeNormalY, planeLocalZ));
        }

        [BurstCompile]
        private void Execute(in LocalToWorld boxLtw, in BoxColliderComponent boxCollider)
        {
            // First pass: collect all colliding targets and their info
            var collidingTargets =
                new NativeList<(int index, BoxCollisionResult collision, LocalToWorld targetLtw, TargetBody targetBody
                    )>(
                    TargetBodyLtwList.Length, Allocator.Temp);

            for (var i = 0; i < TargetBodyLtwList.Length; i++)
            {
                var (targetLtw, bodyTrack) = TargetBodyLtwList[i];
                if (BoxCollisionResult.Try(boxLtw, boxCollider, targetLtw, bodyTrack, out var collision))
                {
                    collidingTargets.Add((i, collision, targetLtw, bodyTrack));
                }
            }

            // Draw box once
            Drawing.WireBox(boxLtw.Position, boxLtw.Rotation, (float3)boxCollider.HalfExtents * 2, BaseBoxColor);

            // Draw all targets (including non-colliding ones)
            for (var i = 0; i < TargetBodyLtwList.Length; i++)
            {
                var (targetLtw, bodyTrack) = TargetBodyLtwList[i];
                bool hasCollision = false;
                BoxCollisionResult collision = default;

                // Check if this target has collision
                for (int j = 0; j < collidingTargets.Length; j++)
                {
                    if (collidingTargets[j].index == i)
                    {
                        hasCollision = true;
                        collision = collidingTargets[j].collision;
                        break;
                    }
                }

                DrawTarget(boxLtw, boxCollider, targetLtw, bodyTrack, i, hasCollision, collision);
            }

            // Draw consolidated collision status for all colliding targets
            if (collidingTargets.Length > 0)
            {
                DrawConsolidatedStatus(boxLtw, collidingTargets);
            }

            collidingTargets.Dispose();
        }

        [BurstCompile]
        private void DrawTarget(
            in LocalToWorld boxLtw, in BoxColliderComponent boxCollider,
            in LocalToWorld targetLtw, in TargetBody targetBody, int targetIndex,
            bool hasCollision, in BoxCollisionResult collision
        )
        {
            float3 targetPosition = targetLtw.Position;
            float3 targetForward = targetLtw.Forward;
            float3 targetTipPosition = targetPosition + targetForward * targetBody.ForwardTip;

            float targetBaseSphereRadius = 0.08f;
            float targetTipSphereRadius = 0.05f;

            // Get target identification color
            Color targetIdColor = TargetIdentificationColors[targetIndex % TargetIdentificationColors.Length];

            // Draw target ID number (always visible)
            float3 targetIdLabelPos = targetPosition + targetLtw.Up * (targetBaseSphereRadius + 0.15f);
            Drawing.Label3D(targetIdLabelPos, EditorCameraRotation, $"T{targetIndex}", LabelSize * 1.2f, targetIdColor);

            if (!hasCollision)
            {
                // Draw basic target gizmo in muted colors when no collision
                Color mutedTargetColor = new Color(targetIdColor.r * 0.5f, targetIdColor.g * 0.5f,
                    targetIdColor.b * 0.5f, 0.6f);
                Drawing.SphereOutline(targetPosition, targetBaseSphereRadius, mutedTargetColor);
                Drawing.Arrow(targetPosition, targetTipPosition, mutedTargetColor);
                return;
            }

            // Handle collision visualization
            float3 boxCenterWorld = boxLtw.Position;
            float3 boxUpWorld = boxLtw.Up;
            float3 boxRightWorld = boxLtw.Right;
            float3 boxHalfExtents = boxCollider.HalfExtents;
            float3 boxTopFaceCenter = boxCenterWorld + boxUpWorld * boxHalfExtents.y;

            Color targetGizmoColor = DefaultTargetColor;

            // Handle tip entered state
            if (collision.IsTipEntered)
            {
                targetGizmoColor = TipEnteredColor;
            }

            // Handle inside state
            if (collision.IsInside)
            {
                targetGizmoColor = InsideColor;
                Drawing.Line(boxCenterWorld, targetPosition, InsideLineColor);
                Drawing.SphereOutline(targetPosition, targetBaseSphereRadius * 1.2f, InsideLineColor);
            }

            // Draw target gizmos with identification color mixed in
            Color finalTargetColor = Color.Lerp(targetGizmoColor, targetIdColor, 0.3f);
            Drawing.SphereOutline(targetPosition, targetBaseSphereRadius, finalTargetColor);
            Drawing.Arrow(targetPosition, targetTipPosition, finalTargetColor);

            float3 targetCenter = targetPosition + targetLtw.Up * targetBody.Leg;
            Drawing.Arrow(targetPosition, targetCenter, finalTargetColor);
            Drawing.SphereOutline(targetTipPosition, targetTipSphereRadius, finalTargetColor);

            float2 topBottomFaceSize = new float2(boxHalfExtents.x * 2, boxHalfExtents.z * 2);
            quaternion topPlaneActualRotation = GetPlaneRotationForAline(boxUpWorld, boxRightWorld);

            // Handle on top of box state
            if (collision.IsOnTopOfBox)
            {
                Drawing.WirePlane(boxTopFaceCenter, topPlaneActualRotation, topBottomFaceSize, OnTopColor);

                float3 standableZoneTopCenter = boxTopFaceCenter + boxUpWorld * targetBody.Leg;
                Drawing.WirePlane(standableZoneTopCenter, topPlaneActualRotation, topBottomFaceSize,
                    StandableZoneColor);

                Color onTopTargetColor = Color.Lerp(OnTopColor, targetIdColor, 0.3f);
                Drawing.SphereOutline(targetPosition, targetBaseSphereRadius * 1.1f, onTopTargetColor);

                // Draw height visualization with individual label
                float3 targetProjectionOnBoxTopSurface = targetPosition - boxUpWorld * collision.ActualHeightOnTop;
                Drawing.Line(targetProjectionOnBoxTopSurface, targetPosition, onTopTargetColor);

                float3 heightLabelPos = (targetProjectionOnBoxTopSurface + targetPosition) * 0.5f +
                                        math.mul(EditorCameraRotation, new float3(LabelSize * 0.5f, 0, 0));
                Drawing.Label3D(heightLabelPos, EditorCameraRotation,
                    $"T{targetIndex}: {collision.ActualHeightOnTop:F2}m",
                    LabelSize * 0.7f, HeightLabelColor);
            }
            else Drawing.WirePlane(boxTopFaceCenter, topPlaneActualRotation, topBottomFaceSize, InactivePlaneColor);
        }

        [BurstCompile]
        private void DrawConsolidatedStatus(
            in LocalToWorld boxLtw,
            in NativeList<(int index, BoxCollisionResult collision, LocalToWorld targetLtw, TargetBody targetBody)>
                collidingTargets
        )
        {
            float3 boxCenterWorld = boxLtw.Position;
            float3 boxUpWorld = boxLtw.Up;
            float targetBaseSphereRadius = 0.08f;

            // Position status labels to the side of the box to avoid overlap
            float3 statusLabelAnchor = boxCenterWorld +
                                       boxLtw.Right * (boxLtw.Value.c0.x * 1.5f) + // Offset to side
                                       boxUpWorld * (targetBaseSphereRadius + TextVerticalOffset + 0.2f);

            int statusLabelCount = 0;
            float labelLineHeight = LabelSize + 0.03f;

            // Header
            Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                EditorCameraRotation, $"Interaction ({collidingTargets.Length})", LabelSize * 0.9f, Color.white);

            // Group by collision type for cleaner display
            var tipEnteredTargets = new NativeList<int>(collidingTargets.Length, Allocator.Temp);
            var insideTargets = new NativeList<int>(collidingTargets.Length, Allocator.Temp);
            var onTopTargets = new NativeList<int>(collidingTargets.Length, Allocator.Temp);

            for (int i = 0; i < collidingTargets.Length; i++)
            {
                var (index, collision, _, _) = collidingTargets[i];
                if (collision.IsTipEntered) tipEnteredTargets.Add(index);
                if (collision.IsInside) insideTargets.Add(index);
                if (collision.IsOnTopOfBox) onTopTargets.Add(index);
            }

            // Display grouped results
            if (tipEnteredTargets.Length > 0)
            {
                string targets = "";
                for (int i = 0; i < tipEnteredTargets.Length; i++)
                {
                    if (i > 0) targets += ", ";
                    targets += $"T{tipEnteredTargets[i]}";
                }

                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                    EditorCameraRotation, $"Tip Entered: {targets}", LabelSize, TipEnteredColor);
            }

            if (insideTargets.Length > 0)
            {
                string targets = "";
                for (int i = 0; i < insideTargets.Length; i++)
                {
                    if (i > 0) targets += ", ";
                    targets += $"T{insideTargets[i]}";
                }

                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                    EditorCameraRotation, $"INSIDE: {targets}", LabelSize, InsideColor);
            }

            if (onTopTargets.Length > 0)
            {
                string targets = "";
                for (int i = 0; i < onTopTargets.Length; i++)
                {
                    if (i > 0) targets += ", ";
                    targets += $"T{onTopTargets[i]}";
                }

                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                    EditorCameraRotation, $"On Top: {targets}", LabelSize, OnTopColor);
            }

            tipEnteredTargets.Dispose();
            insideTargets.Dispose();
            onTopTargets.Dispose();
        }
    }
}
#endif