#if ALINE
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
        public half ForwardTip;

        private void Execute(in LocalToWorld ltw, in BoxColliderComponent boxColliderComponent)
        {
            float3 boxCenterWorld = ltw.Position;
            float3 halfExtents = boxColliderComponent.HalfExtents;

            float3 boxUpWorld = ltw.Up;
            float3 boxForwardWorld = ltw.Forward;

            Drawing.WireBox(ltw.Position, ltw.Rotation, halfExtents * 2, Color.green);


            var targetPosition = TargetLTW.Position;
            float3 playerRelativePosToBoxCenter = targetPosition - boxCenterWorld;

            float localY = math.dot(playerRelativePosToBoxCenter, boxUpWorld);
            float localZ = math.dot(playerRelativePosToBoxCenter, boxForwardWorld);
            var targetLtwForward = TargetLTW.Forward;
            float tipZ = math.dot(playerRelativePosToBoxCenter, targetLtwForward);
            var targetTipForward = targetLtwForward * ForwardTip;
            
            if (tipZ <0 && ForwardTip + halfExtents.z > -tipZ)
            {
                // Debug.Log("Tip entered");
            }

            Drawing.Arrow(targetPosition, targetPosition + targetTipForward);

            if (
                // math.abs(localX) <= halfExtents.x & 
                math.abs(localZ) <= halfExtents.z &
                math.abs(localY) <= halfExtents.y
            )
            {
                Debug.Log($"is INSIDE");
                Drawing.Line(boxCenterWorld, targetPosition);
                Drawing.SphereOutline(targetPosition, 0.1f);
            }

            // Debug.Log($"{localY}, {localZ} TipZ:{tipZ} targetTipForward.z:{targetTipForward.z}");
        }
    }
}
#endif