using Unity.Burst;
using Unity.Entities;


namespace _src.Scripts.Jumps.Editor.Settings
{
    [BurstCompile]
    public partial struct JumpsDebugSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
#if Aline
        
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}