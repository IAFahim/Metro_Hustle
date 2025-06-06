using _src.Scripts.Focuses.Focuses.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Focuses.Focuses.Authoring
{
    public class FocusManagerAuthoring : MonoBehaviour
    {

        private class FocusDynamicBufferBaker : Baker<FocusManagerAuthoring>
        {
            public override void Bake(FocusManagerAuthoring managerAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new FocusCurrentComponent());
            }
        }
    }
}