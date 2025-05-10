using ECSUnitySplineAddon.Runtime.Datas;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.SplineMovement.Runtime.Systems
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    public partial struct MoveAlongSplineSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NativeSplineBlobComponentData>();
        }
        

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            if (state.WorldUnmanaged.Flags == WorldFlags.Editor) deltaTime = 0;

            Entity splineEntity = SystemAPI.GetSingletonEntity<NativeSplineBlobComponentData>();
            NativeSplineBlobComponentData nativeSplineBlobComponentData =
                SystemAPI.GetComponent<NativeSplineBlobComponentData>(splineEntity);
            LocalToWorld splineTransform = SystemAPI.GetComponent<LocalToWorld>(splineEntity);
            var moveAlongIJobEntity = new MoveAlongIJobEntity
            {
                TimeDelta = deltaTime,
                SplineLocalToWorld = splineTransform,
                SplineBlob = nativeSplineBlobComponentData.Value,
            };
            moveAlongIJobEntity.Schedule();
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}