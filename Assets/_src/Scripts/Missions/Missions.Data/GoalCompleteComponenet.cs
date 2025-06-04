using Unity.Entities;

namespace _src.Scripts.Missions.Missions.Data
{
    public struct GoalCompleteComponent : IComponentData
    {
        public byte CompleteBit;
    }
}