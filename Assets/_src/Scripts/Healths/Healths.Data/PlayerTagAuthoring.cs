using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Healths.Healths.Data
{
    public class PlayerTagAuthoring : MonoBehaviour
    {
        public class PlayerTagBaker : Baker<PlayerTagAuthoring>
        {
            public override void Bake(PlayerTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlayerTag>(entity);
            }
        }
    }
}