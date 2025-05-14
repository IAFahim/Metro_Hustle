using _src.Scripts.InputControls.InputControls.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls.Authoring
{
    [DisallowMultipleComponent]
    public class InputThresholdSingletonAuthoring : MonoBehaviour
    {
        public float upThreshold = 0.1f;
        public float sideThreshold = 0.2f;
        public float downThreshold = 0.1f;

        public class UpDownInputThresholdBaker : Baker<InputThresholdSingletonAuthoring>
        {
            public override void Bake(InputThresholdSingletonAuthoring singletonAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new DirectionInputThreshold
                    {
                        UpThreshold = singletonAuthoring.upThreshold, 
                        SideThreshold = singletonAuthoring.sideThreshold, 
                        DownThreshold = singletonAuthoring.downThreshold
                    });
            }
        }
    }
}