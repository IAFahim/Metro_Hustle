using _src.Scripts.Animations.Animations.Data;
using _src.Scripts.Prefabs.Prefabs.Data;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.Animations.Animations
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(EndInitializationEntityCommandBufferSystem))]
    public partial struct InstantiateAnimatorSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            foreach (
                var (animationComponent, localTransform, entity) in
                SystemAPI.Query<AnimatorAssetIndexComponent, RefRO<LocalTransform>>().WithEntityAccess()
            )
            {
                if (false == AssetRequestMonoBehaviour.Instance.TryRequest(
                        animationComponent.Index, localTransform.ValueRO, out var gameObject
                    )) continue;
                ecb.AddComponent(entity, new AnimatorComponent
                {
                    Ref = gameObject.GetComponent<Animator>(),
                    CurrentState = 0,
                    OldState = 0
                });
                ecb.RemoveComponent<AnimatorAssetIndexComponent>(entity);
            }
        }
    }
}