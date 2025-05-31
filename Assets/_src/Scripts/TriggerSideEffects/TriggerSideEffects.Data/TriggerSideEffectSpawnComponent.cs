using System;
using System.Runtime.CompilerServices;
using BovineLabs.Core.ObjectManagement;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data
{
    [BurstCompile]
    public struct TriggerSideEffectSpawnComponent : IComponentData
    {
        public TriggerType TriggerType;

        public ObjectId OnForwardPre;
        public ObjectId OnTop;
        public ObjectId OnInside;
        
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasFlagFast( TriggerType flag) => (TriggerType & flag) != 0;
    }

    [Flags]
    public enum TriggerType : byte
    {
        Nothing = 0,
        HasForward = 0b0000_0001,
        HasTop = 0b0000_0010,
        HasInside = 0b0000_0100,

        EnableForwardPre = 0b0001_0000,
        EnableTop = 0b0010_0000,
        EnableInside = 0b0100_0000,
        
        DestroySelf = 0b1000_0000,

        HasForwardAndEnable = HasForward | EnableForwardPre,
        HasTopAndEnable = HasTop | EnableTop,
        HasInsideAndEnable = HasInside | EnableInside,
        HasAllEnable = HasForwardAndEnable | HasTopAndEnable | HasInsideAndEnable
    }
}