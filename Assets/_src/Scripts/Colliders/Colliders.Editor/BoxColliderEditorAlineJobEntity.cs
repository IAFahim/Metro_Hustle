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
                var (targetEntity, targetLtw, targetBody) = TargetBodyLtwList[i];
                if (BoxCollisionResult.Try(boxLtw, boxCollider, targetLtw, targetBody, out BoxCollisionResult result))
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

                    Draw(boxLtw, boxCollider, targetLtw, targetBody, result);
                }

                statsPair.Dispose();
            }

            // Draw box once
            Drawing.WireBox(boxLtw.Position, boxLtw.Rotation, (float3)boxCollider.HalfExtents * 2, BaseBoxColor);
        }

        private void Draw(
            in LocalToWorld boxLtw, in BoxColliderComponent boxColliderComponent,
            in LocalToWorld targetLtw, in TargetBody targetBody, in BoxCollisionResult result
        )
        {
            // Visualization setup
            float3 boxCenterWorld = boxLtw.Position;
            float3 boxHalfExtents = boxColliderComponent.HalfExtents;
            float3 boxUpWorld = boxLtw.Up;
            float3 boxRightWorld = boxLtw.Right;
            float3 boxForwardWorld = boxLtw.Forward;
            quaternion boxRotation = boxLtw.Rotation;

            float3 targetPosition = targetLtw.Position;
            float3 targetForward = targetLtw.Forward;
            float3 targetTipPosition = targetPosition + targetForward * targetBody.ForwardTip;

            #region Visualization Logic

            float3 boxTopFaceCenter = boxCenterWorld + boxUpWorld * boxHalfExtents.y;

            Color baseBoxColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
            Color targetGizmoColor = new Color(0.5f, 0.7f, 1f);
            float targetBaseSphereRadius = 0.08f;
            float targetTipSphereRadius = 0.05f;

            float3 statusLabelAnchor =
                boxCenterWorld + boxUpWorld * (targetBaseSphereRadius + TextVerticalOffset + 0.2f);
            int statusLabelCount = 0;
            float labelLineHeight = LabelSize + 0.03f;

            Drawing.WireBox(boxCenterWorld, boxRotation, boxHalfExtents * 2, baseBoxColor);

            // Handle tip entered state
            if (result.IsTipEntered)
            {
                targetGizmoColor = new Color(1f, 0.92f, 0.016f);
                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                    EditorCameraRotation, "Tip Entered", LabelSize, targetGizmoColor);
            }

            // Handle inside state
            if (result.IsInside)
            {
                targetGizmoColor = Color.red;
                Drawing.Line(boxCenterWorld, targetPosition, Color.magenta);
                Drawing.SphereOutline(targetPosition, targetBaseSphereRadius * 1.2f, Color.magenta);
                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                    EditorCameraRotation, "INSIDE", LabelSize, targetGizmoColor);
            }

            // Draw target gizmos
            Drawing.SphereOutline(targetPosition, targetBaseSphereRadius, targetGizmoColor);
            Drawing.Arrow(targetPosition, targetTipPosition, targetGizmoColor);

            float3 targetCenter = targetPosition + targetLtw.Up * targetBody.Leg;
            Drawing.Arrow(targetPosition, targetCenter, targetGizmoColor);
            Drawing.SphereOutline(targetTipPosition, targetTipSphereRadius, targetGizmoColor);

            // Setup plane visualizations
            float2 frontBackFaceSize = new float2(boxHalfExtents.x * 2, boxHalfExtents.y * 2);
            float2 topBottomFaceSize = new float2(boxHalfExtents.x * 2, boxHalfExtents.z * 2);

            quaternion topPlaneActualRotation = GetPlaneRotationForAline(boxUpWorld, boxRightWorld);

            // Handle on top of box state
            if (result.IsOnTopOfBox)
            {
                Color aboveColor = new Color(0.8f, 0.4f, 1f);
                Drawing.WirePlane(boxTopFaceCenter, topPlaneActualRotation, topBottomFaceSize, aboveColor);

                float3 standableZoneTopCenter = boxTopFaceCenter + boxUpWorld * targetBody.Leg;
                Drawing.WirePlane(standableZoneTopCenter, topPlaneActualRotation, topBottomFaceSize,
                    new Color(0.5f, 0.7f, 1f, 0.7f));

                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                    EditorCameraRotation, "ON TOP OF BOX", LabelSize, aboveColor);
                Drawing.SphereOutline(targetPosition, targetBaseSphereRadius * 1.1f, aboveColor);

                // Draw height visualization
                float3 targetProjectionOnBoxTopSurface = targetPosition - boxUpWorld * result.ActualHeightOnTop;
                Drawing.Line(targetProjectionOnBoxTopSurface, targetPosition, aboveColor);

                float3 heightLabelPos = (targetProjectionOnBoxTopSurface + targetPosition) * 0.5f +
                                        math.mul(EditorCameraRotation, new float3(LabelSize * 0.5f, 0, 0));
                Drawing.Label3D(heightLabelPos, EditorCameraRotation, $"{result.ActualHeightOnTop:F2}m",
                    LabelSize * 0.8f, Color.white);
            }
            else
            {
                Drawing.WirePlane(boxTopFaceCenter, topPlaneActualRotation, topBottomFaceSize,
                    new Color(0.6f, 0.6f, 0.6f, 0.3f));
            }

            #endregion
        }
    }
}
#endif