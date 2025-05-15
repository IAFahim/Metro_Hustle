using _src.Scripts.InputControls.InputControls.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls.Authoring
{
    [DisallowMultipleComponent]
    public class InputThresholdSingletonAuthoring : MonoBehaviour
    {
        public half minimum = new(5);

        public class UpDownInputThresholdBaker : Baker<InputThresholdSingletonAuthoring>
        {
            public override void Bake(InputThresholdSingletonAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TouchInputThresholdSingleton
                {
                    SideMinimum = authoring.minimum
                });
            }
        }
    }
}