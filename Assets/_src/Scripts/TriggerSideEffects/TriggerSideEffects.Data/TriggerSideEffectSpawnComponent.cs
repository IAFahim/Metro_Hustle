using BovineLabs.Core.ObjectManagement;
using Unity.Entities;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data
{
    public struct TriggerSideEffectSpawnComponent : IComponentData
    {
        public ObjectId OnTop;
        public ObjectId OnInside;
    }
}