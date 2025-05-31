using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Authoring
{
    public class BlockBufferAuthoring : MonoBehaviour
    {
        public float sizeZ = 10;
        public GameObject[] leftPrefabs;
        public GameObject[] rightPrefab;

        private class BlockInfoComponentBaker : Baker<BlockBufferAuthoring>
        {
            public override void Bake(BlockBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new BlockInfoComponent { PerBlockSize = authoring.sizeZ });
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