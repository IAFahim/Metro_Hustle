using _src.Scripts.ZCollisions.ZCollision.Data;
using BovineLabs.Stats.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.ZCollisions.ZCollision.Editor
{
    public struct EntityStatKeyValue
    {
        public Entity Entity;
        public StatKey StatKey;
        public StatValue StatValue;
    }


    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    public partial struct ZCollisionEditorSystem : ISystem
    {
        private BufferLookup<Stat> _statsBufferLookup;
        public NativeQueue<EntityStatKeyValue> StatAddRequest;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _statsBufferLookup = state.GetBufferLookup<Stat>(false);
            StatAddRequest = new(Allocator.Persistent);
        }

        public void OnUpdate(ref SystemState state)
        {
            var collisionEnterEntities = SystemAPI.GetSingletonBuffer<CollisionEnterEntityBuffer>();
            var target = new NativeList<(Entity entity, float3 position)>(Allocator.TempJob);
            _statsBufferLookup.Update(ref state);
            var array = StatAddRequest.ToArray(Allocator.Temp);

            foreach (var collisionEnterEntity in collisionEnterEntities)
            {
                var entity = collisionEnterEntity.Entity;
                var map = _statsBufferLookup[entity].AsMap();
                foreach (var entityStatKeyValue in array)
                {
                    if (entity != entityStatKeyValue.Entity) continue;
                    ref var getOrAdd = ref map.GetOrAddRef(entityStatKeyValue.StatKey, entityStatKeyValue.StatValue);
                    getOrAdd.Added += entityStatKeyValue.StatValue.Added;
                }
                target.Add((entity, SystemAPI.GetComponent<LocalToWorld>(entity).Position));
            }

            StatAddRequest.Clear();

#if ALINE
            var builder = Drawing.DrawingManager.GetBuilder();
            var zCollisionEditorJobEntity = new ZCollisionEditorJobEntity()
            {
                Drawing = builder,
                Target = target.AsParallelReader(),
                StatsLookup = _statsBufferLookup,
                StatAddRequest = StatAddRequest.AsParallelWriter()
            };
            zCollisionEditorJobEntity.Schedule();
            builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            StatAddRequest.Dispose();
        }
    }
}