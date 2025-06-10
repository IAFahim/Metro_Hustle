using System;
using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using Unity.Burst;
using UnityEngine.Serialization;

namespace _src.Scripts.Missions.Missions.Data
{
    [Serializable]
    [BurstCompile]
    public struct Goal
    {
        public bool isComplete;
        public EIntrinsic intrinsic;
        public ObjectiveComparison comparison;
        [FormerlySerializedAs("goal")] public ushort value;


        [BurstCompile]
        public readonly bool TryComplete(int amount, out float progress)
        {
            progress = amount / (float)value;
            return comparison switch
            {
                ObjectiveComparison.GreaterThanOrEqual => value <= amount,
                ObjectiveComparison.LessThanOrEqual => amount <= value,
                ObjectiveComparison.EqualTo => amount == value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}