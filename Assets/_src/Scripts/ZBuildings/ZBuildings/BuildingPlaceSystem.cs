using _src.Scripts.ZBuildings.ZBuildings.Data;
using BovineLabs.Core.Camera;
using BovineLabs.Core.Entropy;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.ZBuildings.ZBuildings
{
    public partial struct BuildingPlaceSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var singletonEntity = SystemAPI.GetSingletonEntity<CameraMain>();
            var cameraZ = SystemAPI.GetComponent<LocalToWorld>(singletonEntity).Value.c3.z;
            var blockInfoComponent = SystemAPI.GetSingletonRW<BlockInfoComponent>();
            var delta = (cameraZ - blockInfoComponent.ValueRO.CurrentCameraZ);
            var distance = blockInfoComponent.ValueRW.PassedCameraZ += delta;
            blockInfoComponent.ValueRW.CurrentCameraZ = cameraZ;
            if (!(distance > blockInfoComponent.ValueRO.PerBlockSize)) return;

            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            var blockLeftBuffer = SystemAPI.GetSingletonBuffer<BlockLeftBuffer>().AsNativeArray();
            var leftLength = blockLeftBuffer.Length;
            var nextInt = GlobalRandom.NextInt(leftLength);
            var position = new float3(-10, 0, cameraZ);
            var instantiate = ecb.Instantiate(blockLeftBuffer[nextInt].Entity);
            var float4X4 = float4x4.TRS(position, quaternion.identity, 1);
            ecb.SetComponent(instantiate, new LocalToWorld()
            {
                Value = float4X4
            });

            // var blockRightBuffer = SystemAPI.GetSingletonBuffer<BlockRightBuffer>().AsNativeArray();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}