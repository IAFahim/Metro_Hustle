using System;
using BovineLabs.Core.ObjectManagement;
using Unity.Entities;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data
{
    public struct TriggerSideEffectSpawnComponent : IComponentData
    {
        public TriggerType TriggerType;

        public ObjectId OnForwardPre;
        public ObjectId OnTop;
        public ObjectId OnInside;
    }

    [Flags]
    public enum TriggerType : byte
    {
        Nothing = 0,
        HasForward = 0b0000_0001,
        HasTop = 0b0000_0010,
        HasInside = 0b0000_0100,

        ForwardPreEnable = 0b0001_0000,
        TopEnable = 0b0010_0000,
        InsideEnable = 0b0100_0000,

        HasForwardAndEnable = HasForward | ForwardPreEnable,
        HasTopAndEnable = HasTop | TopEnable,
        HasInsideAndEnable = HasInside | InsideEnable,

        HasAllEnable = HasForwardAndEnable | HasTopAndEnable | HasInsideAndEnable
    }
}