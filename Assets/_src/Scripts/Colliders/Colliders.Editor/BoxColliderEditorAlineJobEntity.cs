#if ALINE
using _src.Scripts.Colliders.Colliders.Data; // Assuming this is correct
using Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine; // For Color

namespace _src.Scripts.Colliders.Colliders.Editor
{
    public partial struct BoxColliderEditorAlineJobEntity : IJobEntity
    {
        public CommandBuilder Drawing;
        public LocalToWorld TargetLTW; // Player's LocalToWorld
        public half ForwardTip; // How far the player's "tip" projects forward
        
        public quaternion EditorCameraRotation; // Pass active editor camera rotation here

        // Configurable threshold for being "on top"
        private const float OnTopThreshold = 0.1f; // How far above the surface still counts as "on top"

        // Text label properties
        private const float LabelSize = 0.1f; // Adjusted for potentially more labels
        private const float TextVerticalOffset = 0.1f;


        // Helper to create rotation for Aline's XZ-plane from a desired normal (plane's Y) and a desired X-axis for the plane
        private static quaternion GetPlaneRotationForAline(float3 planeNormalY, float3 planeLocalX)
        {
            // Aline's planes are XZ planes, their local Y is their normal.
            // The quaternion should map local X to planeLocalX, local Y to planeNormalY, 
            // and local Z to cross(planeLocalX, planeNormalY).
            float3 planeLocalZ = math.normalize(math.cross(planeLocalX, planeNormalY));
            // Re-orthogonalize planeLocalX in case it wasn't perfectly perpendicular to planeNormalY
            planeLocalX = math.normalize(math.cross(planeNormalY, planeLocalZ));
            return new quaternion(new float3x3(planeLocalX, planeNormalY, planeLocalZ));
        }

        private void Execute(in LocalToWorld ltw, in BoxColliderComponent boxColliderComponent)
        {
            // --- Box Properties ---
            float3 boxCenterWorld = ltw.Position;
            quaternion boxRotation = ltw.Rotation; // This is the box's world rotation
            float3 boxHalfExtents = boxColliderComponent.HalfExtents;
            
            // Box's world axes
            float3 boxUpWorld = math.mul(boxRotation, new float3(0, 1, 0));
            float3 boxForwardWorld = math.mul(boxRotation, new float3(0, 0, 1));
            float3 boxRightWorld = math.mul(boxRotation, new float3(1, 0, 0));

            // --- Target (Player) Properties ---
            float3 targetPosition = TargetLTW.Position;
            quaternion targetRotation = TargetLTW.Rotation;
            float3 targetForward = math.mul(targetRotation, new float3(0,0,1));
            float3 targetTipPosition = targetPosition + targetForward * (float)ForwardTip;

            // --- Relative Position & Projections ---
            float3 targetRelativePosToBoxCenter = targetPosition - boxCenterWorld;
            float localY = math.dot(targetRelativePosToBoxCenter, boxUpWorld);
            float localZBox = math.dot(targetRelativePosToBoxCenter, boxForwardWorld);
            float projectionOnTargetForward = math.dot(targetRelativePosToBoxCenter, targetForward);

            // --- Key Points on the Box ---
            float3 boxTopFaceCenter = boxCenterWorld + boxUpWorld * boxHalfExtents.y;
            float3 boxFrontFaceCenter = boxCenterWorld + boxForwardWorld * boxHalfExtents.z;
            float3 boxBackFaceCenter = boxCenterWorld - boxForwardWorld * boxHalfExtents.z;

            // --- State Calculations ---
            bool isTipEntered = false;
            bool isInsideSimplified = false;
            bool isInFrontOfBox = false;
            bool isBehindBox = false;
            bool isAboveBox = false;
            bool isOnTopOfBox = false;
            float actualHeightOnTop = 0f;

            if (projectionOnTargetForward < 0 && ((float)ForwardTip + boxHalfExtents.z) > -projectionOnTargetForward)
                isTipEntered = true;

            if (math.abs(localY) <= boxHalfExtents.y && math.abs(projectionOnTargetForward) <= boxHalfExtents.z)
                isInsideSimplified = true;

            if (localZBox > boxHalfExtents.z) isInFrontOfBox = true;
            if (localZBox < -boxHalfExtents.z) isBehindBox = true;
            if (localY > boxHalfExtents.y) isAboveBox = true;

            if (
                math.abs(localZBox) <= boxHalfExtents.z &&
                localY >= boxHalfExtents.y && localY <= boxHalfExtents.y + OnTopThreshold)
            {
                isOnTopOfBox = true;
                actualHeightOnTop = localY - boxHalfExtents.y; // Player's base height above box's top surface
            }
            
            float topHeightValue = boxTopFaceCenter.y;

            // --- VISUALIZATION REGION ---
            #region Visualization Logic
            // Base colors and sizes
            Color baseBoxColor = new Color(0.5f, 0.5f, 0.5f, 0.4f); 
            Color targetGizmoColor = new Color(0.5f, 0.7f, 1f); 
            float targetBaseSphereRadius = 0.08f;
            float targetTipSphereRadius = 0.05f;

            // Label setup
            float3 statusLabelAnchor = targetPosition + boxUpWorld * (targetBaseSphereRadius + TextVerticalOffset + 0.2f); // Anchor point above target
            int statusLabelCount = 0;
            float labelLineHeight = LabelSize + 0.03f;

            // Draw the Base Box (subdued)
            Drawing.WireBox(boxCenterWorld, boxRotation, boxHalfExtents * 2, baseBoxColor);

            // --- Target States and Gizmos ---
            if (isTipEntered)
            {
                targetGizmoColor = new Color(1f, 0.92f, 0.016f); // Yellow
                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight, EditorCameraRotation, "Tip Entered", LabelSize, targetGizmoColor);
            }

            if (isInsideSimplified)
            {
                targetGizmoColor = Color.red;
                Drawing.Line(boxCenterWorld, targetPosition, Color.magenta);
                Drawing.SphereOutline(targetPosition, targetBaseSphereRadius * 1.2f, Color.magenta);
                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight, EditorCameraRotation, "INSIDE (Simplified)", LabelSize, targetGizmoColor);
            }

            Drawing.SphereOutline(targetPosition, targetBaseSphereRadius, targetGizmoColor);
            Drawing.Arrow(targetPosition, targetTipPosition, targetGizmoColor);
            Drawing.SphereOutline(targetTipPosition, targetTipSphereRadius, targetGizmoColor);

            // --- Box Face/Zone Highlights ---
            // Size for front/back faces (Width along boxRight, Height along boxUp)
            float2 frontBackFaceSize = new float2(boxHalfExtents.x * 2, boxHalfExtents.y * 2);
            // Size for top/bottom faces (Width along boxRight, Depth along boxForward)
            float2 topBottomFaceSize = new float2(boxHalfExtents.x * 2, boxHalfExtents.z * 2);

            // Plane rotations
            quaternion frontPlaneActualRotation = GetPlaneRotationForAline(boxForwardWorld, boxRightWorld);
            quaternion backPlaneActualRotation = GetPlaneRotationForAline(-boxForwardWorld, boxRightWorld);
            quaternion topPlaneActualRotation = GetPlaneRotationForAline(boxUpWorld, boxRightWorld);


            if (isInFrontOfBox)
            {
                Drawing.WirePlane(boxFrontFaceCenter, frontPlaneActualRotation, frontBackFaceSize, Color.cyan);
                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight, EditorCameraRotation, "Target In Front", LabelSize, Color.cyan);
            }

            if (isBehindBox)
            {
                Color behindColor = new Color(1f, 0.65f, 0f); // Orange
                Drawing.WirePlane(boxBackFaceCenter, backPlaneActualRotation, frontBackFaceSize, behindColor);
                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight, EditorCameraRotation, "Target Behind", LabelSize, behindColor);
            }
            
            if (isOnTopOfBox)
            {
                Drawing.WirePlane(boxTopFaceCenter, topPlaneActualRotation, topBottomFaceSize, Color.blue);
                float3 standableZoneTopCenter = boxTopFaceCenter + boxUpWorld * OnTopThreshold;
                Drawing.WirePlane(standableZoneTopCenter, topPlaneActualRotation, topBottomFaceSize, new Color(0.5f, 0.7f, 1f, 0.7f)); 

                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight, EditorCameraRotation, "ON TOP OF BOX", LabelSize, Color.blue);
                Drawing.SphereOutline(targetPosition, targetBaseSphereRadius * 1.1f, Color.blue);

                // Visualize player's height on top
                float3 targetProjectionOnBoxTopSurface = targetPosition - boxUpWorld * actualHeightOnTop;
                Drawing.Line(targetProjectionOnBoxTopSurface, targetPosition, Color.blue);
                
                float3 heightLabelPos = (targetProjectionOnBoxTopSurface + targetPosition) * 0.5f + math.mul(EditorCameraRotation, new float3(LabelSize * 0.5f, 0, 0)); // Offset sideways from line mid-point
                Drawing.Label3D(heightLabelPos, EditorCameraRotation, $"{actualHeightOnTop:F2}m", LabelSize * 0.8f, Color.white);

            }
            else if (isAboveBox) 
            {
                Color aboveColor = new Color(0.8f, 0.4f, 1f); // Lavender
                Drawing.WirePlane(boxTopFaceCenter, topPlaneActualRotation, topBottomFaceSize, new Color(aboveColor.r, aboveColor.g, aboveColor.b, 0.5f));
                Drawing.Label3D(statusLabelAnchor + boxUpWorld * statusLabelCount++ * labelLineHeight, EditorCameraRotation, "Target Above Box", LabelSize, aboveColor);
            }
            else
            {
                // Default subtle outline for the top surface if no other top-related state
                Drawing.WirePlane(boxTopFaceCenter, topPlaneActualRotation, topBottomFaceSize, new Color(0.6f, 0.6f, 0.6f, 0.3f));
            }

            // Visualize topHeight (the box's top surface Y-coordinate)
            float3 topHeightLabelPos = boxTopFaceCenter + boxUpWorld * (OnTopThreshold + TextVerticalOffset + 0.05f);
            string topHeightText = $"Box Top Y: {topHeightValue:F2}";
            Drawing.Label3D(topHeightLabelPos, EditorCameraRotation, topHeightText, LabelSize * 0.9f, Color.gray);
            #endregion
            // --- END VISUALIZATION REGION ---
        }
    }
}
#endif