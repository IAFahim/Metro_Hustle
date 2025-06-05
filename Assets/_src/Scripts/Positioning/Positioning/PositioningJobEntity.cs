using _src.Scripts.Positioning.Positioning.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.Positioning.Positioning
{
    public partial struct PositioningJobEntity : IJobEntity
    {
        public float DeltaTime;

        private void Execute(
            ref LocalToWorld ltw,
            ref LeftRightComponent leftRight,
            in ForwardBackComponent forwardBack,
            ref GravityComponent gravity,
            in HeightComponent height
        )
        {
            ltw.Value.c3.z += forwardBack.Offset * DeltaTime;

            if (ltw.Value.c3.z > height.Value)
            {
                gravity.Velocity -= (half)(gravity.Gravity * DeltaTime * gravity.GMultiplier);
                ltw.Value.c3.y += gravity.Velocity * DeltaTime;
            }

            if (leftRight.Direction == 0) return;
            {
                float movement = leftRight.Step * leftRight.Direction * DeltaTime;
                float newX = ltw.Value.c3.x + movement;
                float newCurrent = leftRight.Current + movement;

                if (leftRight.Direction > 0)
                {
                    if (newCurrent >= leftRight.Target)
                    {
                        float overshoot = newCurrent - leftRight.Target;
                        newX -= overshoot;
                        newCurrent = leftRight.Target;
                        leftRight.Direction = (half)0;
                    }
                }
                else
                {
                    if (newCurrent <= leftRight.Target)
                    {
                        float overshoot = leftRight.Target - newCurrent;
                        newX += overshoot;
                        newCurrent = leftRight.Target;
                        leftRight.Direction = (half)0;
                    }
                }

                ltw.Value.c3.x = newX;
                leftRight.Current = (half)newCurrent;
            }
        }
    }
}