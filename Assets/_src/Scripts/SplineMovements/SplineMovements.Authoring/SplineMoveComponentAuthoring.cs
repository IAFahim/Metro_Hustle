using _src.Scripts.SplineMovements.SplineMovements.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.SplineMovements.SplineMovements.Authoring
{
    public class SplineMoveComponentAuthoring : MonoBehaviour
    {
        public byte curveIndex;
        public half speed = new(2);
        public half sideOffset;
        public half distance;

        public class SplineMoveComponentBaker : Baker<SplineMoveComponentAuthoring>
        {
            public override void Bake(SplineMoveComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SplineMoveComponent
                {
                    CurveIndex = authoring.curveIndex,
                    Speed = authoring.speed,
                    SideOffset = authoring.sideOffset,
                    Distance = authoring.distance
                });
            }
        }
    }
}