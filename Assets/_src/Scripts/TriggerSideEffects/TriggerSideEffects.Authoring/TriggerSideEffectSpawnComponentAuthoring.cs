using _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data;
using BovineLabs.Core.ObjectManagement;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Authoring
{
    internal class TriggerSideEffectSpawnComponentAuthoring : MonoBehaviour
    {
        public ObjectId onTop;
        public ObjectId onInside;

        private class TriggerSideEffectComponentSpawnBaker : Baker<TriggerSideEffectSpawnComponentAuthoring>
        {
            public override void Bake(TriggerSideEffectSpawnComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TriggerSideEffectSpawnComponent
                {
                    OnTop = authoring.onTop, OnInside = authoring.onInside
                });
            }
        }
    }
}