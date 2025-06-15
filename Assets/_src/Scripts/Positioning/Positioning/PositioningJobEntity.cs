using _src.Scripts.Positioning.Positioning.Data;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.Positioning.Positioning
{
    public partial struct PositioningJobEntity : IJobEntity
    {
        public float DeltaTime;
        [ReadOnly] public ComponentLookup<FaceMoveDirectionTag> FaceMoveDirectionTagLookup;

        private void Execute(
            Entity entity,
            ref LocalToWorld ltw,
            ref GravityComponent gravity,
            ref LeftRightComponent leftRight,
            in ForwardBackComponent forwardBack,
            in HeightComponent height
        )
        {
            if (gravity.GMultiplier != 0)
            {
                gravity.Velocity += (half)(gravity.Gravity * DeltaTime * gravity.GMultiplier);
                ltw.Value.c3.y += gravity.Velocity * DeltaTime;
            }

            if (ltw.Value.c3.y < height.Value)
            {
                ltw.Value.c3.y = height.Value;
                gravity.GMultiplier = 0;
                gravity.Velocity = new half(0);
            }

            if (FaceMoveDirectionTagLookup.HasComponent(entity))
            {
                if (forwardBack.Speed < 0)
                {
                    var c0X = math.abs(ltw.Value.c0.x);
                    ltw.Value.c0.x = -c0X;
                    ltw.Value.c2.z = -c0X;
                }
            }

            ltw.Value.c3.z += forwardBack.Speed * DeltaTime;

            var currentX = ltw.Value.c3.x;
            float direction = leftRight.GetDirection(currentX);
            if (Hint.Likely(direction == 0 || leftRight.Speed == 0)) return;
            {
                float offset = leftRight.Speed * direction * DeltaTime;
                float newX = currentX + offset;
                float newCurrent = newX;
                if (direction > 0)
                {
                    if (newX > leftRight.Target) newCurrent = leftRight.Target;
                }
                else
                {
                    if (newCurrent < leftRight.Target) newCurrent = leftRight.Target;
                }

                ltw.Value.c3.x = newCurrent;
            }
        }
    }
}