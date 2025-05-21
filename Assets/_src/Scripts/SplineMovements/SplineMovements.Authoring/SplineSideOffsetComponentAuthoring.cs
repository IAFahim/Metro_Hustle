using _src.Scripts.Easings.Runtime.Datas;
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
        public half easingT;
        public Ease ease;
        
        

        private class SplineMoveOffsetBaker : Baker<SplineSideOffsetComponentAuthoring>
        {
            public override void Bake(SplineSideOffsetComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SplineSideOffsetComponent
                {
                    StartOffset = authoring.startOffset,
                    EndOffset = authoring.endOffset,
                    EasingT = authoring.easingT,
                    Ease = authoring.ease,
                });
            }
        }
    }
}