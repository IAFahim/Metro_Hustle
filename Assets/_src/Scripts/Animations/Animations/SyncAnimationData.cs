using _src.Scripts.Animations.Animations.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.Animations.Animations
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct SyncAnimationData : ISystem
    {
        private static readonly int StateHash = Animator.StringToHash("state");
        private static readonly int Mirror = Animator.StringToHash("mirror");

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (animatorComponent, localToWorld) in SystemAPI
                         .Query<RefRW<AnimatorComponent>, RefRO<LocalToWorld>>())
            {
                var position = localToWorld.ValueRO.Position;
                var rotation = localToWorld.ValueRO.Rotation;
                animatorComponent.ValueRO.Ref.Value.transform.SetPositionAndRotation(
                    position,
                    rotation
                );
                var currentState = animatorComponent.ValueRO.CurrentState;
                var oldState = animatorComponent.ValueRO.OldState;
                if (currentState == oldState) return;
                
                animatorComponent.ValueRW.OldState = currentState;
                if (currentState < 0 && oldState > 0) animatorComponent.ValueRO.Ref.Value.SetBool(Mirror, true);
                else if (currentState > 0 && oldState < 0) animatorComponent.ValueRO.Ref.Value.SetBool(Mirror, false);
                animatorComponent.ValueRO.Ref.Value.SetInteger(StateHash, math.abs(currentState));
            }
        }
    }
}