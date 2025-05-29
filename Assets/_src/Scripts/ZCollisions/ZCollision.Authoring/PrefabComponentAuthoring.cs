using _src.Scripts.ZCollisions.ZCollision.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.ZCollisions.ZCollision.Authoring
{
    internal class PrefabComponentAuthoring : MonoBehaviour
    {
        public GameObject prefab;

        private class PrefabComponentBaker : Baker<PrefabComponentAuthoring>
        {
            public override void Bake(PrefabComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PrefabComponent
                {
                    Prefab = GetEntity(authoring.prefab, TransformUsageFlags.None)
                });
            }
        }
    }
}