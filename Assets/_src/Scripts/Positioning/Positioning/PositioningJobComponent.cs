using _src.Scripts.Positioning.Positioning.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.Positioning.Positioning
{
    public partial struct PositioningJobComponent : IJobEntity
    {
        public float DeltaTime;

        private void Execute(
            ref LocalToWorld ltw,
            in LeftRightComponent leftRight,
            in ForwardBackComponent forwardBack,
            ref GravityEnableComponent gravity,
            in HeightComponent height
        )
        {
            ltw.Value.c3.x += leftRight.Offset * DeltaTime;
            ltw.Value.c3.z += forwardBack.Offset * DeltaTime;
            ltw.Value.c3.y += height.Offset * DeltaTime;
            if (gravity.Enable)
            {
                gravity.Velocity -= (half)(gravity.Gravity * DeltaTime);
                ltw.Value.c3.y += gravity.Velocity * DeltaTime;
            }
            else gravity.Velocity = new half(0);
        }
    }
}