using _src.Scripts.Positioning.Positioning.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Positioning.Positioning.Authoring
{
    public class PositioningAuthoring : MonoBehaviour
    {
        public bool gravityEnable;
        public half gravity = new(30);
        public half velocity;
        public half heightIncrease;
        public half leftRightOffset;
        public half forwardBackOffset;

        public class LeftRightOffsetBaker : Baker<PositioningAuthoring>
        {
            public override void Bake(PositioningAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new LeftRightComponent { Step = authoring.leftRightOffset });
                AddComponent(entity, new ForwardBackComponent { Offset = authoring.forwardBackOffset });
                AddComponent(entity, new GravityEnableComponent()
                {
                    Enable = authoring.gravityEnable,
                    Gravity = authoring.gravity,
                    Velocity = authoring.velocity
                });
                AddComponent<HeightComponent>(entity, new()
                {
                    Offset = authoring.heightIncrease
                });
            }
        }
    }
}