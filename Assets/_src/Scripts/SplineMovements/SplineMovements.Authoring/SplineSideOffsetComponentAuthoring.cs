using _src.Scripts.SplineMovements.SplineMovements.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.SplineMovements.SplineMovements.Authoring
{
    public class SplineSideOffsetComponentAuthoring : MonoBehaviour
    {
        public half startOffset;
        public half endOffset;
        public half speed = new(1);
        
        

        private class SplineMoveOffsetBaker : Baker<SplineSideOffsetComponentAuthoring>
        {
            public override void Bake(SplineSideOffsetComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SplineSideOffsetComponent
                {
                    CurrentOffset = authoring.startOffset,
                    EndOffset = authoring.endOffset,
                    Speed = authoring.speed,
                });
            }
        }
    }
}