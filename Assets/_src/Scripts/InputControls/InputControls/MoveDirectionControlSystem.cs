using BovineLabs.Core.Input;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls
{
    public partial struct MoveDirectionControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        
        public void OnUpdate(ref SystemState state)
        {
            var inputComponentEntity = SystemAPI.GetSingletonEntity<InputComponent>();
            var inputComponent = SystemAPI.GetComponent<InputComponent>(inputComponentEntity);
            var upInput = inputComponent.MoveDelta.y > 0;
            var downInput = inputComponent.MoveDelta.y < 0;
            Debug.Log($"upInput: {upInput} downInput: {downInput}");
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}