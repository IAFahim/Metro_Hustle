using _src.Scripts.Colliders.Colliders.Data;
using BovineLabs.Stats.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.Colliders.Colliders.Editor
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    public partial struct SphereColliderEditorSystem : ISystem
    {
        private NativeList<(Entity entity, float3 position, float range)> _array;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TrackCollidableEntityBuffer>();
            _array = new(Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
#if ALINE
            var builder = Drawing.DrawingManager.GetBuilder();
            _array.Clear();
            foreach (var entityBuffer in SystemAPI.GetSingletonBuffer<TrackCollidableEntityBuffer>())
            {
                var entity = entityBuffer.Entity;
                var rangeKey = new StatKey { Value = (ushort)EStat.RangeSq };
                var range = SystemAPI.GetBuffer<Stat>(entity).AsMap().GetOrDefault(rangeKey);
                _array.Add((
                    entity,
                    SystemAPI.GetComponent<LocalToWorld>(entity).Position,
                    range.Value
                ));
            }

            var sphereColliderEditorJobEntity = new SphereColliderEditorJobEntity
            {
                TargetInfos = _array.AsParallelReader(),
                CommandBuilder = builder
            };
            sphereColliderEditorJobEntity.ScheduleParallel();
            builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            if (_array.IsCreated) _array.Dispose();
        }
    }
}