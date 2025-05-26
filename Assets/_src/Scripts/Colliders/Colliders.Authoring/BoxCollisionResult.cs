using _src.Scripts.Colliders.Colliders.Data;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    [BurstCompile]
    public struct BoxCollisionResult
    {
        public bool IsTipEntered;
        public bool IsInside;
        public bool IsOnTopOfBox;
        public half ActualHeightOnTop;

        [BurstCompile]
        public static bool Try(
            in LocalToWorld boxLtw, in BoxColliderComponent boxCollider,
            in LocalToWorld targetLtw, in TargetBody targetBody,
            out BoxCollisionResult collisionResult
        )
        {
            float3 targetRelativePosToBoxCenter = targetLtw.Position - boxLtw.Position;
            float3 boxHalfExtents = boxCollider.HalfExtents;
            float projectionOnTargetForward = math.dot(targetRelativePosToBoxCenter, targetLtw.Forward);

            collisionResult = new BoxCollisionResult
            {
                IsTipEntered = false,
                IsInside = false,
                IsOnTopOfBox = false,
                ActualHeightOnTop = new half()
            };


            // Check if tip entered
            if (projectionOnTargetForward < 0 && targetBody.ForwardTip + boxHalfExtents.z > -projectionOnTargetForward)
            {
                collisionResult.IsTipEntered = true;
            }

            var localX = math.dot(targetRelativePosToBoxCenter, boxLtw.Right);
            var localY = math.dot(targetRelativePosToBoxCenter, boxLtw.Up);

            bool insideX = math.abs(localX) <= boxHalfExtents.x;
            bool insideY = math.abs(localY) <= boxHalfExtents.y;
            bool insideZ = math.abs(projectionOnTargetForward) <= boxHalfExtents.z;

            if (insideX && insideY && insideZ) collisionResult.IsInside = true;

            var actualHeightOnTop = boxHalfExtents.y - localY;
            if (
                insideX && insideZ &&
                localY < boxHalfExtents.y + targetBody.Leg &&
                actualHeightOnTop < targetBody.Leg
            )
            {
                collisionResult.ActualHeightOnTop = (half)actualHeightOnTop;
                collisionResult.IsOnTopOfBox = true;
            }

            return collisionResult.IsTipEntered || collisionResult.IsInside || collisionResult.IsOnTopOfBox;
        }
    }
}