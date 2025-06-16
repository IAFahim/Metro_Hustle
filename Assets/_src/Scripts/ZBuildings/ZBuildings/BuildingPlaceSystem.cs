using _src.Scripts.ZBuildings.ZBuildings.Data;
using BovineLabs.Core.Camera;
using BovineLabs.Core.Entropy;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.ZBuildings.ZBuildings
{
    public partial struct BuildingPlaceSystem : ISystem
    {
        private static readonly float[] Road = { -2.5f, 0, 2.5f };

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

                CreateBlock(ref ecb, zOffset, sideOffset, blockBuffer.Left, blockBuffer.Road, blockBuffer.Right,
                    obsticalBuffers[GlobalRandom.NextInt(obsticalBuffers.Length)].Entity,
                    new float3(Road[GlobalRandom.NextInt(3)], 0, zOffset)
                );
                createLogic.ValueRW.Progress = (half)0;
                createLogic.ValueRW.CreateCount++;
            }
            else createLogic.ValueRW.Progress += (half)delta;

            if (Hint.Likely(createLogic.ValueRO.PreWarmed)) return;
            var blockBuffersBatch = SystemAPI.GetSingletonBuffer<BlockBuffer>();
            var obsticalBuffersBatch = SystemAPI.GetSingletonBuffer<ObsticalBuffer>();
            var ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var stationData = SystemAPI.GetSingleton<StationData>();
            CreateStation(ref ECB, 0, stationData.StartBlock, stationData.EndRoad);
            createLogic.ValueRW.CreateCount++;

            for (float offset = cameraZ;
                 offset < createLogic.ValueRO.AHeadCreate;
                 offset += perBlockSize)
            {
                var length = blockBuffersBatch.Length;
                var random = GlobalRandom.NextInt(length);
                var z = cameraZ + createLogic.ValueRO.CreateCount * perBlockSize;
                var blockBuffer = blockBuffersBatch[random];

                CreateBlock(ref ECB, z, sideOffset, blockBuffer.Left, blockBuffer.Road, blockBuffer.Right,
                    obsticalBuffersBatch[GlobalRandom.NextInt(obsticalBuffersBatch.Length)].Entity,
                    new float3(Road[GlobalRandom.NextInt(3)], 0, z)
                );
                createLogic.ValueRW.CreateCount++;
            }

            createLogic.ValueRW.PreWarmed = true;
        }


        private void CreateStation(ref EntityCommandBuffer ecb, float3 position,
            Entity station, Entity road)
        {
            ecb.SetComponent(ecb.Instantiate(station), new LocalTransform
            {
                Position = position, Rotation = quaternion.identity, Scale = 1
            });

            ecb.SetComponent(ecb.Instantiate(road), new LocalTransform
            {
                Position = position, Rotation = quaternion.identity, Scale = 1
            });
        }

        private void CreateBlock(ref EntityCommandBuffer ecb, float zPosition, float sideOffset,
            Entity left, Entity road, Entity right, Entity obstacle, float3 obstaclePosition)
        {
            var leftPosition = new float3(-sideOffset, 0, zPosition);
            ecb.SetComponent(ecb.Instantiate(left), new LocalTransform
            {
                Position = leftPosition, Rotation = new quaternion(0, 1, 0, 0), Scale = 1
            });


            ecb.SetComponent(ecb.Instantiate(obstacle), new LocalToWorld
            {
                Value = float4x4.TRS(obstaclePosition, quaternion.identity, 1)
            });

            var roadPosition = new float3(0, 0, zPosition);
            ecb.SetComponent(ecb.Instantiate(road), new LocalTransform
            {
                Position = roadPosition, Rotation = quaternion.identity, Scale = 1
            });

            // Create right building
            var rightPosition = new float3(sideOffset, 0, zPosition);
            ecb.SetComponent(ecb.Instantiate(right), new LocalTransform
            {
                Position = rightPosition, Rotation = quaternion.identity, Scale = 1
            });
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}