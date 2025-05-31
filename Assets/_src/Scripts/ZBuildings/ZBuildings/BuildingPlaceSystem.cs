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

            var perBlockSize = createLogic.ValueRO.PerBlockSize;
            var sideOffset = createLogic.ValueRO.SideOffset;
            if (createLogic.ValueRO.Progress >= perBlockSize)
            {
                var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);
                var blockLeftBuffer = SystemAPI.GetSingletonBuffer<BlockLeftBuffer>();
                var leftLength = blockLeftBuffer.Length;
                
                var nextInt = GlobalRandom.NextInt(leftLength);
                var position = new float3(-sideOffset, 0, perBlockSize * createLogic.ValueRO.CreateCount);
                var instantiate = ecb.Instantiate(blockLeftBuffer[nextInt].Entity);
                ecb.SetComponent(instantiate, new LocalTransform()
                {
                    Position = position,
                    Rotation = quaternion.Euler(0, 180, 0),
                    Scale = 1
                });
                createLogic.ValueRW.Progress = (half)0;
                createLogic.ValueRW.CreateCount++;
            }
            else createLogic.ValueRW.Progress += (half)delta;

            if (createLogic.ValueRO.PreWarmed) return;

            for (float offset = cameraZ;
                 offset < createLogic.ValueRO.AHeadCreate;
                 offset += perBlockSize)
            {
                var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);
                var blockLeftBuffer = SystemAPI.GetSingletonBuffer<BlockLeftBuffer>().AsNativeArray();
                var leftLength = blockLeftBuffer.Length;
                var nextInt = GlobalRandom.NextInt(leftLength);
                var position = new float3(-sideOffset, 0, cameraZ + createLogic.ValueRO.CreateCount * perBlockSize);
                var instantiate = ecb.Instantiate(blockLeftBuffer[nextInt].Entity);
                ecb.SetComponent(instantiate, new LocalTransform()
                {
                    Position = position,
                    Rotation = quaternion.Euler(0, 180, 0),
                    Scale = 1
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