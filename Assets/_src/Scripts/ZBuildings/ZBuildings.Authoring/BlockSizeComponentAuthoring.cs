using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Authoring
{
    public class BlockBufferAuthoring : MonoBehaviour
    {
        public half sizeZ = new(10);
        public half ahead = new(200);
        public GameObject[] leftPrefabs;
        public GameObject[] rightPrefab;

        private class BlockInfoComponentBaker : Baker<BlockBufferAuthoring>
        {
            public override void Bake(BlockBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new BlockCreateLogicComponent
                {
                    PerBlockSize = authoring.sizeZ,
                    AHeadCreate = authoring.ahead,
                });
                var blockLeftBuffers = AddBuffer<BlockLeftBuffer>(entity);
                foreach (var obj in authoring.leftPrefabs)
                {
                    blockLeftBuffers.Add(new BlockLeftBuffer
                    {
                        Entity = GetEntity(obj, TransformUsageFlags.None)
                    });
                }

                var blockRightBuffers = AddBuffer<BlockRightBuffer>(entity);
                foreach (var obj in authoring.rightPrefab)
                {
                    blockRightBuffers.Add(new BlockRightBuffer
                    {
                        Entity = GetEntity(obj, TransformUsageFlags.None)
                    });
                }
            }
        }
    }
}