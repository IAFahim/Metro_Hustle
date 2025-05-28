using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Authoring
{
    public class BuildingGapComponentAuthoring : MonoBehaviour
    {
        public half start;
        public half end;

        public class BuildingGapComponentBaker : Baker<BuildingGapComponentAuthoring>
        {
            public override void Bake(BuildingGapComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingGapComponent { Start = authoring.start, End = authoring.end });
            }
        }
    }
}