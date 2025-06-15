using _src.Scripts.Easings.Easings.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Easings.Easings.Authoring
{
    internal class EaseComponentAuthoring : MonoBehaviour
    {
        public bool enable;
        public half duration = new(1);
        public half elapsed;
        public Ease ease = Ease.Linear;

        internal class EasePositioningBaker : Baker<EaseComponentAuthoring>
        {
            public override void Bake(EaseComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EaseComponent
                {
                    Duration = authoring.duration,
                    Elapsed = authoring.elapsed,
                    Ease = authoring.ease
                });
                SetComponentEnabled<EaseComponent>(entity, authoring.enable);
            }
        }
    }
}