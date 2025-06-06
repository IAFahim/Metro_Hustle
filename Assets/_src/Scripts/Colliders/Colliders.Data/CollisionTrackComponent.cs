using _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data.enums;
using Unity.Entities;

namespace _src.Scripts.Colliders.Colliders.Data
{
    public struct CollisionTrackComponent : IComponentData
    {
        public ESideEffect SideEffect;
    }
}