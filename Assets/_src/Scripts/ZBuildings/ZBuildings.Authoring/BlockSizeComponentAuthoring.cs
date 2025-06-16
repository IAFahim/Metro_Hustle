using System;
using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace _src.Scripts.ZBuildings.ZBuildings.Authoring
{
    internal class BlockBufferAuthoring : MonoBehaviour
    {
        public half perBlockSize = new(10);
        public half ahead = new(200);
        public half roadSize = new(10);
        [FormerlySerializedAs("segment50")] public Building[] blocks;
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
                    SideOffset = authoring.roadSize
                });
                var blockLeftBuffers = AddBuffer<BlockBuffer>(entity);
                foreach (var block in authoring.blocks)
                {
                    blockLeftBuffers.Add(new BlockBuffer
                    {
                        Left = GetEntity(block.left, TransformUsageFlags.None),
                        Road = GetEntity(authoring.road, TransformUsageFlags.None),
                        Right = GetEntity(block.right, TransformUsageFlags.None)
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

        [Serializable]
        internal class Building
        {
            public GameObject left;
            public GameObject right;
        }
    }
}