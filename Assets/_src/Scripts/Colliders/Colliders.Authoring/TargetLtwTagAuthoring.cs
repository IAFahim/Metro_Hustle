using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Colliders.Colliders.Authoring
{
    public class TargetLtwTagAuthoring : MonoBehaviour
    {
        public half ForwardTip;

        public class TargetLtwTagBaker : Baker<TargetLtwTagAuthoring>
        {
            public override void Bake(TargetLtwTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TargetLtwTag { ForwardTip = authoring.ForwardTip });
            }
        }
    }
}