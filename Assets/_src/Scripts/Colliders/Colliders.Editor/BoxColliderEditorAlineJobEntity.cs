#if ALINE
using _src.Scripts.Colliders.Colliders.Authoring;
using _src.Scripts.Colliders.Colliders.Data;
using BovineLabs.Core;
using BovineLabs.Stats.Data;
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
        [ReadOnly] public BufferLookup<Stat> StatLookup;
        public NativeQueue<IStatsBuffer>.ParallelWriter IStatsBuffer;

        [ReadOnly]
        public NativeArray<(Entity entity, LocalToWorld targetLtw, TargetBody targetBody)>.ReadOnly TargetBodyLtwList;

        // Color management
        private static readonly Color BaseBoxColor = new(0.5f, 0.5f, 0.5f, 0.4f);
        private static readonly Color DefaultTargetColor = new(0.5f, 0.7f, 1f);
        private static readonly Color TipEnteredColor = new(1f, 0.92f, 0.016f);
        private static readonly Color InsideColor = Color.red;
        private static readonly Color InsideLineColor = Color.magenta;
        private static readonly Color OnTopColor = new(0.8f, 0.4f, 1f);
        private static readonly Color StandableZoneColor = new(0.5f, 0.7f, 1f, 0.7f);
        private static readonly Color InactivePlaneColor = new(0.6f, 0.6f, 0.6f, 0.3f);
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
        private void Execute(Entity entity, in LocalToWorld boxLtw, in BoxColliderComponent boxCollider)
        {
            if (!StatLookup.TryGetBuffer(entity, out var statsBuffer)) return;
            var statsMap = statsBuffer.AsMap();
            var statsPair = statsMap.GetKeyValueArrays(Allocator.Temp);
            var singleStatsKey = statsPair.Keys;
            var singleStatsValue = statsPair.Values;
            for (var i = 0; i < TargetBodyLtwList.Length; i++)
            {
                var (targetEntity, targetLtw, bodyTrack) = TargetBodyLtwList[i];
                if (BoxCollisionResult.Try(boxLtw, boxCollider, targetLtw, bodyTrack, out var collision))
                {
                    for (int j = 0; j < statsPair.Length; j++)
                    {
                        IStatsBuffer.Enqueue(new IStatsBuffer()
                        {
                            Entity = targetEntity,
                            Key = singleStatsKey[i],
                            Value = singleStatsValue[i].Value
                        });
                    }
                    DrawTarget(boxLtw, boxCollider, targetLtw, bodyTrack, i, collision);
                }

                statsPair.Dispose();
            }

            // Draw box once
            Drawing.WireBox(boxLtw.Position, boxLtw.Rotation, (float3)boxCollider.HalfExtents * 2, BaseBoxColor);
        }

        [BurstCompile]
        private void DrawTarget(
            in LocalToWorld boxLtw, in BoxColliderComponent boxCollider,
            in LocalToWorld targetLtw, in TargetBody targetBody, int targetIndex, in BoxCollisionResult collision
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

            if (!collision.HasInteraction)
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
    }
}
#endif