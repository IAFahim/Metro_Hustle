using _src.Scripts.Focuses.Focuses.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Focuses.Focuses.Authoring
{
    public class FocusComponentAuthoring : MonoBehaviour
    {
        public byte priority = 0;

        public class FocusComponentBaker : Baker<FocusComponentAuthoring>
        {
            public override void Bake(FocusComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new FocusComponent { Priority = authoring.priority });
            }
        }
    }
}