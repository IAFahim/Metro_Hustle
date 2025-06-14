using _src.Scripts.Colliders.Colliders.Data;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.Colliders.Colliders.Editor
{
    public partial struct SphereColliderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
#if ALINE
            // SystemAPI.GetSingletonBuffer<TrackCollidableEntityBuffer>()
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}