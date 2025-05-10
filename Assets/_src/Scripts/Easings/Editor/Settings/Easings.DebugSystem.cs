#if Aline
using Drawing;
#endif


using Unity.Burst;
using Unity.Entities;

namespace Scripts.Easings.Editor
{
    [BurstCompile]
    public partial struct EasingsDebugSystem : ISystem
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