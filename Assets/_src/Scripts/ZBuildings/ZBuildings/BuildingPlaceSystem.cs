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
            var createLogic = SystemAPI.GetSingletonRW<BlockCreateLogicComponent>();

            var delta = (cameraZ - createLogic.ValueRO.PassedCameraZ);
            createLogic.ValueRW.PassedCameraZ = cameraZ;

            if (createLogic.ValueRO.Progress >= createLogic.ValueRO.PerBlockSize)
            {
                var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);
                var blockLeftBuffer = SystemAPI.GetSingletonBuffer<BlockLeftBuffer>();
                var leftLength = blockLeftBuffer.Length;
                
                var nextInt = GlobalRandom.NextInt(leftLength);
                var position = new float3(-4.5f, 0, createLogic.ValueRO.PerBlockSize * createLogic.ValueRO.CreateCount);
                var instantiate = ecb.Instantiate(blockLeftBuffer[nextInt].Entity);
                var float4X4 = float4x4.TRS(position, quaternion.identity, 1);
                ecb.SetComponent(instantiate, new LocalToWorld()
                {
                    Value = float4X4
                });
                createLogic.ValueRW.Progress = (half)0;
                createLogic.ValueRW.CreateCount++;
            }
            else createLogic.ValueRW.Progress += (half)delta;

            if (createLogic.ValueRO.PreWarmed) return;

            for (float offset = cameraZ;
                 offset < createLogic.ValueRO.AHeadCreate;
                 offset += createLogic.ValueRO.PerBlockSize)
            {
                var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);
                var blockLeftBuffer = SystemAPI.GetSingletonBuffer<BlockLeftBuffer>().AsNativeArray();
                var leftLength = blockLeftBuffer.Length;
                var nextInt = GlobalRandom.NextInt(leftLength);
                var position = new float3(-4.5f, 0, cameraZ + offset);
                var instantiate = ecb.Instantiate(blockLeftBuffer[nextInt].Entity);
                var float4X4 = float4x4.TRS(position, quaternion.identity, 1);
                ecb.SetComponent(instantiate, new LocalToWorld()
                {
                    Value = float4X4
                });
                createLogic.ValueRW.PreWarmed = true;
                createLogic.ValueRW.CreateCount++;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}