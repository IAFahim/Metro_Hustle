using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    [SelectionBase]
    public class TargetLtwTagAuthoring : MonoBehaviour
    {
        public half leg = new(1);
        public half forwardTip = new(1);

        public class TargetLtwTagBaker : Baker<TargetLtwTagAuthoring>
        {
            public override void Bake(TargetLtwTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TargetBody
                {
                    ForwardTip = authoring.forwardTip,
                    Leg = authoring.leg
                });
            }
        }
    }
}