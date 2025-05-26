#if ALINE
using _src.Scripts.Colliders.Colliders.Authoring;
using _src.Scripts.Colliders.Colliders.Data;
using Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Editor
{
    // Separate collision result struct

    public partial struct BoxColliderEditorAlineJobEntity : IJobEntity
    {
        public CommandBuilder Drawing;
        public LocalToWorld TargetLtw;
        public TargetTrack TargetTrack;
        public quaternion EditorCameraRotation;

        private const float LabelSize = 0.1f;
        private const float TextVerticalOffset = 0.1f;

        private static quaternion GetPlaneRotationForAline(float3 planeNormalY, float3 planeLocalX)
        {
            float3 planeLocalZ = math.normalize(math.cross(planeLocalX, planeNormalY));
            planeLocalX = math.normalize(math.cross(planeNormalY, planeLocalZ));
            return new quaternion(new float3x3(planeLocalX, planeNormalY, planeLocalZ));
        }

        private void Execute(in LocalToWorld ltw, in BoxColliderComponent boxColliderComponent)
        {
            // Calculate collision once
            bool isCollision =
                BoxCollisionResult.Calculate(boxColliderComponent, ltw, TargetLtw, TargetTrack, out var collision);

            // Visualization setup
            float3 boxCenterWorld = ltw.Position;
            float3 boxHalfExtents = boxColliderComponent.HalfExtents;
            float3 boxUpWorld = ltw.Up;
            float3 boxRightWorld = ltw.Right;
            float3 boxForwardWorld = ltw.Forward;
            quaternion boxRotation = ltw.Rotation;

            float3 targetPosition = TargetLtw.Position;
            float3 targetForward = TargetLtw.Forward;
            float3 targetTipPosition = targetPosition + targetForward * TargetTrack.ForwardTip;

            #region Visualization Logic

            Color baseBoxColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
            Drawing.WireBox(boxCenterWorld, boxRotation, boxHalfExtents * 2, baseBoxColor);
            if (!isCollision) return;

            float3 boxTopFaceCenter = boxCenterWorld + boxUpWorld * boxHalfExtents.y;
            Color targetGizmoColor = new Color(0.5f, 0.7f, 1f);
            float targetBaseSphereRadius = 0.08f;
            float targetTipSphereRadius = 0.05f;

            float3 statusLabelAnchor =
                boxCenterWorld + boxUpWorld * (targetBaseSphereRadius + TextVerticalOffset + 0.2f);
            int statusLabelCount = 0;
            float labelLineHeight = LabelSize + 0.03f;


            // Handle tip entered state
            if (collision.IsTipEntered)
            {
                targetGizmoColor = new Color(1f, 0.92f, 0.016f);
                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                    EditorCameraRotation, "Tip Entered", LabelSize, targetGizmoColor);
            }

            // Handle inside state
            if (collision.IsInside)
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

            float3 targetCenter = targetPosition + TargetLtw.Up * TargetTrack.Leg;
            Drawing.Arrow(targetPosition, targetCenter, targetGizmoColor);
            Drawing.SphereOutline(targetTipPosition, targetTipSphereRadius, targetGizmoColor);

            float2 topBottomFaceSize = new float2(boxHalfExtents.x * 2, boxHalfExtents.z * 2);


            quaternion topPlaneActualRotation = GetPlaneRotationForAline(boxUpWorld, boxRightWorld);
            // Handle on top of box state
            if (collision.IsOnTopOfBox)
            {
                Color aboveColor = new Color(0.8f, 0.4f, 1f);
                Drawing.WirePlane(boxTopFaceCenter, topPlaneActualRotation, topBottomFaceSize, aboveColor);

                float3 standableZoneTopCenter = boxTopFaceCenter + boxUpWorld * TargetTrack.Leg;
                Drawing.WirePlane(standableZoneTopCenter, topPlaneActualRotation, topBottomFaceSize,
                    new Color(0.5f, 0.7f, 1f, 0.7f));

                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                    EditorCameraRotation, "ON TOP OF BOX", LabelSize, aboveColor);
                Drawing.SphereOutline(targetPosition, targetBaseSphereRadius * 1.1f, aboveColor);

                // Draw height visualization
                float3 targetProjectionOnBoxTopSurface = targetPosition - boxUpWorld * collision.ActualHeightOnTop;
                Drawing.Line(targetProjectionOnBoxTopSurface, targetPosition, aboveColor);

                float3 heightLabelPos = (targetProjectionOnBoxTopSurface + targetPosition) * 0.5f +
                                        math.mul(EditorCameraRotation, new float3(LabelSize * 0.5f, 0, 0));
                Drawing.Label3D(heightLabelPos, EditorCameraRotation, $"{collision.ActualHeightOnTop:F2}m",
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