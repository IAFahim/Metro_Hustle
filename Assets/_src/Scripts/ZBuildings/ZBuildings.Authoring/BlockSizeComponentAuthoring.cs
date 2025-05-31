using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Authoring
{
    public class BlockBufferAuthoring : MonoBehaviour
    {
        public half perBlockSize = new(10);
        public half ahead = new(200);
        public half sizeOffset = new(10);
        public GameObject[] leftPrefabs;
        public GameObject road;
        public GameObject[] obsticals;

        private class BlockInfoComponentBaker : Baker<BlockBufferAuthoring>
        {
            public override void Bake(BlockBufferAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new BlockCreateLogicComponent
                {
                    PerBlockSize = authoring.perBlockSize,
                    AHeadCreate = authoring.ahead,
                    SideOffset = authoring.sizeOffset
                });
                var blockLeftBuffers = AddBuffer<BlockBuffer>(entity);
                for (var i = 0; i < authoring.leftPrefabs.Length; i++)
                {
                    var obj = authoring.leftPrefabs[i];
                    blockLeftBuffers.Add(new BlockBuffer
                    {
                        Left = GetEntity(obj, TransformUsageFlags.None),
                        Road = GetEntity(authoring.road, TransformUsageFlags.None),
                        Right = GetEntity(obj, TransformUsageFlags.None)
                    });
                }


                var obsticalBuffers = AddBuffer<ObsticalBuffer>(entity);
                foreach (var obstical in authoring.obsticals)
                {
                    obsticalBuffers.Add(new ObsticalBuffer()
                    {
                        Entity = GetEntity(obstical, TransformUsageFlags.None)
                    });
                }
            }
        }
    }
}