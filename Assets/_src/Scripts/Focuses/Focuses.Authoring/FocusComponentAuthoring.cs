using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Focuses.Focuses.Data
{
    public class FocusComponentAuthoring : MonoBehaviour
    {
        public byte Priority = 0;

        public class FocusComponentBaker : Baker<FocusComponentAuthoring>
        {
            public override void Bake(FocusComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FocusComponent { Priority = authoring.Priority });
            }
        }
    }
}