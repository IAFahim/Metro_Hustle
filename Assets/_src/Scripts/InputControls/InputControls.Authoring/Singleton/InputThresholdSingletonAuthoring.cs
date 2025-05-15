using _src.Scripts.InputControls.InputControls.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls.Authoring.Singleton
{
    [DisallowMultipleComponent]
    public class InputThresholdSingletonAuthoring : MonoBehaviour
    {
        public half horizontal = new(0.005);
        public half vertical  = new(0.01);

        public class UpDownInputThresholdBaker : Baker<InputThresholdSingletonAuthoring>
        {
            public override void Bake(InputThresholdSingletonAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TouchInputThresholdSingleton
                {
                    Horizontal = authoring.horizontal,
                    Vertical = authoring.vertical
                });
            }
        }
    }
}