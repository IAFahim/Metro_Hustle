using _src.Scripts.CinemachineLink.CinemachineLink.Data;
using _src.Scripts.Focuses.Focuses.Data;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.CinemachineLink.CinemachineLink
{
    public partial struct CinemachineLinkerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var entity = SystemAPI.GetSingleton<FocusCurrentComponent>().Entity;
            var ltwCurrent = SystemAPI.GetComponent<LocalToWorld>(entity); 
            CinemachineLinkerSingleton.Transform.SetLocalPositionAndRotation(
                ltwCurrent.Position,
                ltwCurrent.Rotation
            );
        }
    }
}