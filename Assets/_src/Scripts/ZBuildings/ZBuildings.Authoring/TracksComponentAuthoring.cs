using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Authoring
{
    public class TracksComponentAuthoring : MonoBehaviour
    {
        public GameObject prefab;

        public class TracksComponentBaker : Baker<TracksComponentAuthoring>
        {
            public override void Bake(TracksComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new TrackComponent
                {
                    Prefab = GetEntity(authoring.prefab, TransformUsageFlags.Renderable)
                });
            }
        }
    }
}