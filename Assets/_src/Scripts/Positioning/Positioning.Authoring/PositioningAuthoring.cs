using _src.Scripts.Positioning.Positioning.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace _src.Scripts.Positioning.Positioning.Authoring
{
    [RequireComponent(typeof(HeightComponentAuthoring))]
    [DisallowMultipleComponent]
    public class PositioningAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("leftRightOffset")]
        [Header("X")] public half leftRightSpeed;
        public half leftRightTarget;
        
        [Header("Y")] public half gravity = new(-30);
        public half velocity;
        public byte gMultiplier;

        [FormerlySerializedAs("forwardBackOffset")] [Header("Z")] public half forwardBackSpeed;

        public class LeftRightOffsetBaker : Baker<PositioningAuthoring>
        {
            public override void Bake(PositioningAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new LeftRightComponent
                {
                    Speed = authoring.leftRightSpeed ,
                    Target = authoring.leftRightTarget
                });
                
                AddComponent(entity, new GravityComponent
                {
                    Gravity = authoring.gravity,
                    Velocity = authoring.velocity,
                    GMultiplier = authoring.gMultiplier
                });
                
                AddComponent(entity, new ForwardBackComponent { Speed = authoring.forwardBackSpeed });
            }
        }
    }
}