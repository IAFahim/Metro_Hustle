using _src.Scripts.Focuses.Focuses.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Focuses.Focuses.Authoring
{
    public class FocusSingletonAuthoring : MonoBehaviour
    {

        private class FocusDynamicBufferBaker : Baker<FocusSingletonAuthoring>
        {
            public override void Bake(FocusSingletonAuthoring singletonAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new FocusSingletonComponent());
            }
        }
    }
}