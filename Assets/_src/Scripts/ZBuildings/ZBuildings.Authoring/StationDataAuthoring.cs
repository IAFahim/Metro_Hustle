using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Authoring
{
    public class StationDataAuthoring : MonoBehaviour
    {
        public GameObject startBlock;
        public GameObject startRoad;
        public GameObject endBlock;
        public GameObject endRoad;

        public class StationDataBaker : Baker<StationDataAuthoring>
        {
            public override void Bake(StationDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new StationData
                {
                    StartBlock = GetEntity(authoring.startBlock, TransformUsageFlags.Renderable),
                    StartRoad = GetEntity(authoring.startRoad, TransformUsageFlags.Renderable),
                    EndBlock = GetEntity(authoring.endBlock, TransformUsageFlags.Renderable),
                    EndRoad = GetEntity(authoring.endRoad, TransformUsageFlags.Renderable)
                });
            }
        }
    }
}