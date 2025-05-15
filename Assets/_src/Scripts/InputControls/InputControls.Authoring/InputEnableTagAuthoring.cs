using _src.Scripts.InputControls.InputControls.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls.Authoring
{
    public class InputEnableTagAuthoring : MonoBehaviour
    {
        public bool enable = true;

        private class InputEnableTagBaker : Baker<InputEnableTagAuthoring>
        {
            public override void Bake(InputEnableTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<InputEnableTag>(entity);
                SetComponentEnabled<InputEnableTag>(entity, authoring.enable);
            }
        }
    }
}