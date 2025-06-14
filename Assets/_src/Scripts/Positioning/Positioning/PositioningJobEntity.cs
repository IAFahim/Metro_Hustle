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

            if (leftRight.Direction == 0) return;
            {
                float movement = leftRight.Speed * leftRight.Direction * DeltaTime;
                float newX = ltw.Value.c3.x + movement;
                float newCurrent = leftRight.Current + movement;

                if (leftRight.Direction > 0)
                {
                    if (newCurrent >= leftRight.Target)
                    {
                        float overshoot = newCurrent - leftRight.Target;
                        newX -= overshoot;
                        newCurrent = leftRight.Target;
                        leftRight.Direction = 0;
                    }
                }
                else
                {
                    if (newCurrent <= leftRight.Target)
                    {
                        float overshoot = leftRight.Target - newCurrent;
                        newX += overshoot;
                        newCurrent = leftRight.Target;
                        leftRight.Direction = 0;
                    }
                }

                ltw.Value.c3.x = newX;
                leftRight.Current = (half)newCurrent;
            }
        }
    }
}