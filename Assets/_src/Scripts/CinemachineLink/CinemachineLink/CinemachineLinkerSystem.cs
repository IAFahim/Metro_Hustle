using _src.Scripts.CinemachineLink.CinemachineLink.Data;
using _src.Scripts.Focuses.Focuses.Data;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.CinemachineLink.CinemachineLink
{
    public partial struct CinemachineLinkerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FocusManagerCurrentInfoComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var focusManagerEntity = SystemAPI.GetSingletonEntity<FocusManagerCurrentInfoComponent>();
            var currentInfoComponent = SystemAPI.GetComponent<FocusManagerCurrentInfoComponent>(focusManagerEntity);
            CinemachineLinkerSingleton.Target.SetLocalPositionAndRotation(
                currentInfoComponent.Position,
                currentInfoComponent.Rotation
            );
        }
    }
}