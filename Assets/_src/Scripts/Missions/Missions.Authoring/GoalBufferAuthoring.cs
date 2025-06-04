using _src.Scripts.Missions.Missions.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Missions.Missions.Authoring
{
    public class GoalBufferAuthoring : MonoBehaviour
    {
        public GoalBuffer[] goals;
        public byte goalCompleteBit;

        public class GoalBufferBaker : Baker<GoalBufferAuthoring>
        {
            public override void Bake(GoalBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var goalBuffers = AddBuffer<GoalBuffer>(entity);
                foreach (var goal in authoring.goals) goalBuffers.Add(goal);
                AddComponent(entity, new GoalCompleteComponent
                {
                    CompleteBit = authoring.goalCompleteBit
                });
            }
        }
    }
}