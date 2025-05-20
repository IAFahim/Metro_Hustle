using _src.Scripts.Easings.Runtime.Datas;
using _src.Scripts.SplineMovements.SplineMovements.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.SplineMovements.SplineMovements.Authoring
{
    public class SplineSideOffsetComponentAuthoring : MonoBehaviour
    {
        public half sideOffset;
        public half targetOffset;
        public Ease Ease;

        private class SplineMoveOffsetBaker : Baker<SplineSideOffsetComponentAuthoring>
        {
            public override void Bake(SplineSideOffsetComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SplineSideOffsetComponent
                {
                    SideOffset = authoring.sideOffset,
                    TargetSideOffset = authoring.targetOffset,
                    SideEase = authoring.Ease,
                });
            }
        }
    }
}