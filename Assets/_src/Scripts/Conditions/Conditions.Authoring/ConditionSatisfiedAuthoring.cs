using _src.Scripts.Conditions.Conditions.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Conditions.Conditions.Authoring
{
    internal class ConditionSatisfiedAuthoring : MonoBehaviour
    {
        private class ConditionSatisfiedBaker : Baker<ConditionSatisfiedAuthoring>
        {
            public override void Bake(ConditionSatisfiedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<ConditionSatisfied>(entity);
                SetComponentEnabled<ConditionSatisfied>(entity, false);
            }
        }
    }
}