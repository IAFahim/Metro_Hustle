using System;
using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using Unity.Entities;
using UnityEngine.Serialization;

namespace _src.Scripts.Missions.Missions.Data
{
    [Serializable]
    [InternalBufferCapacity(0)]
    public struct GoalBuffer : IBufferElementData
    {
        public EIntrinsic intrinsic;
        public ushort goal;

        public bool IsComplete(int amount) => goal <= amount;
    }
}