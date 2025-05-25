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
    public partial struct BoxColliderEditorAlineJobEntity : IJobEntity
    {
        public CommandBuilder Drawing;
        public LocalToWorld TargetLTW;
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
            float3 boxCenterWorld = ltw.Position;
            float3 boxHalfExtents = boxColliderComponent.HalfExtents;

            float3 boxUpWorld = ltw.Up;
            float3 boxRightWorld = ltw.Right;

            float3 targetPosition = TargetLTW.Position;
            float3 targetForward = TargetLTW.Forward;
            float3 targetTipPosition = targetPosition + targetForward * TargetTrack.ForwardTip;

            float3 targetRelativePosToBoxCenter = targetPosition - boxCenterWorld;
            float localY = math.dot(targetRelativePosToBoxCenter, boxUpWorld);
            float localX = math.dot(targetRelativePosToBoxCenter, boxRightWorld);
            float projectionOnTargetForward = math.dot(targetRelativePosToBoxCenter, targetForward);

            float3 boxTopFaceCenter = boxCenterWorld + boxUpWorld * boxHalfExtents.y;

            bool isTipEntered = false;
            bool isInside = false;
            bool isOnTopOfBox = false;

            if (
                projectionOnTargetForward < 0 &&
                TargetTrack.ForwardTip + boxHalfExtents.z > -projectionOnTargetForward
            ) isTipEntered = true;

            var insideZ = math.abs(projectionOnTargetForward) <= boxHalfExtents.z;
            var insideX = math.abs(localX) <= boxHalfExtents.x;
            if (
                math.abs(localY) <= boxHalfExtents.y &&
                insideZ &&
                insideX
            ) isInside = true;

            if (insideZ && insideX && localY < boxHalfExtents.y + TargetTrack.Leg)
            {
                isOnTopOfBox = true;
            }


            #region Visualization Logic
            
            float3 boxForwardWorld = ltw.Forward;
            quaternion boxRotation = ltw.Rotation;

            float actualHeightOnTop = 0f;
            if (isOnTopOfBox)
            {
                actualHeightOnTop = localY - boxHalfExtents.y;
            }

            Color baseBoxColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
            Color targetGizmoColor = new Color(0.5f, 0.7f, 1f);
            float targetBaseSphereRadius = 0.08f;
            float targetTipSphereRadius = 0.05f;

            float3 statusLabelAnchor =
                boxCenterWorld + boxUpWorld * (targetBaseSphereRadius + TextVerticalOffset + 0.2f);
            int statusLabelCount = 0;
            float labelLineHeight = LabelSize + 0.03f;

            Drawing.WireBox(boxCenterWorld, boxRotation, boxHalfExtents * 2, baseBoxColor);

            if (isTipEntered)
            {
                targetGizmoColor = new Color(1f, 0.92f, 0.016f);
                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                    EditorCameraRotation, "Tip Entered", LabelSize, targetGizmoColor);
            }

            if (isInside)
            {
                targetGizmoColor = Color.red;
                Drawing.Line(boxCenterWorld, targetPosition, Color.magenta);
                Drawing.SphereOutline(targetPosition, targetBaseSphereRadius * 1.2f, Color.magenta);
                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                    EditorCameraRotation, "INSIDE", LabelSize, targetGizmoColor);
            }

            Drawing.SphereOutline(targetPosition, targetBaseSphereRadius, targetGizmoColor);
            Drawing.Arrow(targetPosition, targetTipPosition, targetGizmoColor);

            float3 targetCenter = targetPosition + TargetLTW.Up * TargetTrack.Leg;
            Drawing.Arrow(targetPosition, targetCenter, targetGizmoColor);
            Drawing.SphereOutline(targetTipPosition, targetTipSphereRadius, targetGizmoColor);

            float2 frontBackFaceSize = new float2(boxHalfExtents.x * 2, boxHalfExtents.y * 2);
            float2 topBottomFaceSize = new float2(boxHalfExtents.x * 2, boxHalfExtents.z * 2);

            quaternion frontPlaneActualRotation = GetPlaneRotationForAline(boxForwardWorld, boxRightWorld);
            quaternion backPlaneActualRotation = GetPlaneRotationForAline(-boxForwardWorld, boxRightWorld);
            quaternion topPlaneActualRotation = GetPlaneRotationForAline(boxUpWorld, boxRightWorld);


            if (isOnTopOfBox)
            {
                Color aboveColor = new Color(0.8f, 0.4f, 1f);
                Drawing.WirePlane(boxTopFaceCenter, topPlaneActualRotation, topBottomFaceSize, aboveColor);
                float3 standableZoneTopCenter = boxTopFaceCenter + boxUpWorld * TargetTrack.Leg;
                Drawing.WirePlane(standableZoneTopCenter, topPlaneActualRotation, topBottomFaceSize,
                    new Color(0.5f, 0.7f, 1f, 0.7f));

                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight,
                    EditorCameraRotation, "ON TOP OF BOX", LabelSize, aboveColor);
                Drawing.SphereOutline(targetPosition, targetBaseSphereRadius * 1.1f, aboveColor);

                float3 targetProjectionOnBoxTopSurface = targetPosition - boxUpWorld * actualHeightOnTop;
                Drawing.Line(targetProjectionOnBoxTopSurface, targetPosition, aboveColor);

                float3 heightLabelPos = (targetProjectionOnBoxTopSurface + targetPosition) * 0.5f +
                                        math.mul(EditorCameraRotation, new float3(LabelSize * 0.5f, 0, 0));
                Drawing.Label3D(heightLabelPos, EditorCameraRotation, $"{actualHeightOnTop:F2}m", LabelSize * 0.8f,
                    Color.white);
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