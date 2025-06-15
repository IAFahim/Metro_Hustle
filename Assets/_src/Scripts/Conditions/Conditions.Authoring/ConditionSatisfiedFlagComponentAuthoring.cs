using _src.Scripts.Conditions.Conditions.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Conditions.Conditions.Authoring
{
    internal class ConditionSatisfiedFlagComponentAuthoring : MonoBehaviour
    {
        public byte flag;

        private class ConditionSatisfiedByteComponentBaker : Baker<ConditionSatisfiedFlagComponentAuthoring>
        {
            public override void Bake(ConditionSatisfiedFlagComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ConditionSatisfiedFlagComponent { Flag = authoring.flag });
            }
        }
    }
}