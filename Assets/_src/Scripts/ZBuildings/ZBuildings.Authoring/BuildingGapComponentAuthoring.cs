using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Authoring
{
    public class BuildingGapComponentAuthoring : MonoBehaviour
    {
        public half forward = new(1);
        public half backword = new(1);

        public class BuildingGapComponentBaker : Baker<BuildingGapComponentAuthoring>
        {
            public override void Bake(BuildingGapComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new BuildingGapComponent
                {
                    Forward = authoring.forward, Backward = authoring.backword
                });
            }
        }
    }
}