using System;
using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.Missions.Missions.Data
{
    [Serializable]
    [InternalBufferCapacity(0)]
    [BurstCompile]
    public struct GoalBuffer : IBufferElementData
    {
        public bool isComplete;
        public EIntrinsic intrinsic;
        public ObjectiveComparison comparison;
        public ushort goal;

        
        
        [BurstCompile]
        public readonly bool TryComplete(int amount)
        {
            if (isComplete) return true;
            return comparison switch
            {
                ObjectiveComparison.GreaterThanOrEqual => goal <= amount,
                ObjectiveComparison.LessThanOrEqual => amount <= goal,
                ObjectiveComparison.EqualTo => amount == goal,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}