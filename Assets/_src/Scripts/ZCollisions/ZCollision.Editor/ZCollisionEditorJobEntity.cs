using _src.Scripts.ZCollisions.ZCollision.Data;
using BovineLabs.Stats.Data;
using Drawing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace _src.Scripts.ZCollisions.ZCollision.Editor
{
    [BurstCompile]
    [WithNone(typeof(CollisionEnterComponent))]
    public partial struct ZCollisionEditorJobEntity : IJobEntity
    {
        public CommandBuilder Drawing;
        [ReadOnly] public NativeArray<(Entity entity, float3 position)>.ReadOnly Target;
        private static readonly Color BoxColor = Color.darkSeaGreen;
        [ReadOnly] public BufferLookup<Stat> StatsLookup;

        [WriteOnly]
        public NativeQueue<EntityStatKeyValue>.ParallelWriter StatAddRequest;


        [BurstCompile]
        private void Execute(Entity entity, in WorldRenderBounds worldRender)
        {
            if (!StatsLookup.TryGetBuffer(entity, out var buffer)) return;
            var kvPairs = buffer.AsMap();
            var center = worldRender.Value.Center;
            foreach (var (targetEntity, position) in Target)
            {
                if (worldRender.Value.Contains(position))
                {
                    Drawing.Line(center, position);
                    foreach (var kvPair in kvPairs)
                    {
                        StatAddRequest.Enqueue(new EntityStatKeyValue()
                        {
                            Entity = targetEntity,
                            StatKey = kvPair.Key,
                            StatValue = kvPair.Value
                        });
                    }
                }
            }

            Drawing.WireBox(center, worldRender.Value.Extents * 2, BoxColor);
        }
    }
}