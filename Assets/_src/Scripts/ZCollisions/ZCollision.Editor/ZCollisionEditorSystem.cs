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
            var prefab = SystemAPI.GetSingleton<PrefabComponent>();
            var target = new NativeList<(Entity entity, float3 position)>(Allocator.TempJob);
            _statsBufferLookup.Update(ref state);
            var array = StatAddRequest.ToArray(Allocator.Temp);

            foreach (var collisionEnterEntity in collisionEnterEntities)
            {
                var entity = collisionEnterEntity.Entity;
                target.Add((entity, SystemAPI.GetComponent<LocalToWorld>(entity).Position));
                
            }
            
            StatAddRequest.Clear();

            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            quaternion editorCamRot = quaternion.identity;
            if (UnityEditor.SceneView.lastActiveSceneView != null)
            {
                editorCamRot = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
            }
            
#if ALINE
            var builder = Drawing.DrawingManager.GetBuilder();
            var zCollisionEditorJobEntity = new ZCollisionEditorJobEntity()
            {
                Drawing = builder,
                Target = target.AsParallelReader(),
                EditorCameraRotation = editorCamRot,
                // StatsLookup = _statsBufferLookup,
                // StatAddRequest = StatAddRequest.AsParallelWriter(),
                // Prefab = prefab.Prefab,
                // ECB = ecb.AsParallelWriter()
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