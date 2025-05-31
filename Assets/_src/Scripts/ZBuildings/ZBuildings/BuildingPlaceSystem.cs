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
        public static readonly float[] Road = { -2.5f, 0, 2.5f };

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
                var blockBuffers = SystemAPI.GetSingletonBuffer<BlockBuffer>();
                var obsticalBuffers = SystemAPI.GetSingletonBuffer<ObsticalBuffer>();
                
                var length = blockBuffers.Length;
                var random = GlobalRandom.NextInt(length);
                
                var zOffset = perBlockSize * createLogic.ValueRO.CreateCount;
                var position = new float3(-sideOffset, 0, zOffset);
                var blockBuffer = blockBuffers[random];
                ecb.SetComponent(ecb.Instantiate(blockBuffer.Left), new LocalTransform()
                {
                    Position = position,
                    Rotation = quaternion.Euler(0, 180, 0),
                    Scale = 1
                });
                
                var obsticleRoad = GlobalRandom.NextInt(3);
                position = new float3(Road[obsticleRoad], 0, zOffset);
                var obsticle = obsticalBuffers[GlobalRandom.NextInt(obsticalBuffers.Length)].Entity;
                ecb.SetComponent(ecb.Instantiate(obsticle), new LocalToWorld()
                {
                    Value = float4x4.TRS(position, quaternion.identity, 1)
                });

                position = new float3(0, 0, zOffset);
                ecb.SetComponent(ecb.Instantiate(blockBuffer.Road), new LocalToWorld()
                {
                    Value = float4x4.TRS(position, quaternion.identity, 1)
                });

                position = new float3(sideOffset, 0, zOffset);
                ecb.SetComponent(ecb.Instantiate(blockBuffer.Right), new LocalTransform()
                {
                    Position = position,
                    Rotation = quaternion.identity,
                    Scale = 1
                });
                createLogic.ValueRW.Progress = (half)0;
                createLogic.ValueRW.CreateCount++;
            }
            else createLogic.ValueRW.Progress += (half)delta;

            if (createLogic.ValueRO.PreWarmed) return;
            var blockBuffersBatch = SystemAPI.GetSingletonBuffer<BlockBuffer>();
            var obsticalBuffersBatch = SystemAPI.GetSingletonBuffer<ObsticalBuffer>();
            var ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);


            for (float offset = cameraZ;
                 offset < createLogic.ValueRO.AHeadCreate;
                 offset += perBlockSize)
            {
                var length = blockBuffersBatch.Length;
                var random = GlobalRandom.NextInt(length);
                var z = cameraZ + createLogic.ValueRO.CreateCount * perBlockSize;
                var blockBuffer = blockBuffersBatch[random];

                var position = new float3(-sideOffset, 0, z);
                ECB.SetComponent(ECB.Instantiate(blockBuffer.Left), new LocalTransform()
                {
                    Position = position,
                    Rotation = quaternion.Euler(0, 180, 0),
                    Scale = 1
                });

                var obsticleRoad = GlobalRandom.NextInt(3);
                position = new float3(Road[obsticleRoad], 0, z);
                var obsticle = obsticalBuffersBatch[GlobalRandom.NextInt(obsticalBuffersBatch.Length)].Entity;
                ECB.SetComponent(ECB.Instantiate(obsticle), new LocalToWorld()
                {
                    Value = float4x4.TRS(position, quaternion.identity, 1)
                });

                position = new float3(0, 0, z);
                ECB.SetComponent(ECB.Instantiate(blockBuffer.Road), new LocalToWorld()
                {
                    Value = float4x4.TRS(position, quaternion.identity, 1)
                });

                position = new float3(sideOffset, 0, z);
                ECB.SetComponent(ECB.Instantiate(blockBuffer.Right), new LocalTransform()
                {
                    Position = position,
                    Rotation = quaternion.identity,
                    Scale = 1
                });

                createLogic.ValueRW.CreateCount++;
            }

            createLogic.ValueRW.PreWarmed = true;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}