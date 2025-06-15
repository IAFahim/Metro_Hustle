using _src.Scripts.Easings.Runtime.Datas;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Easings.Easings.Authoring
{
    internal class EaseComponentAuthoring : MonoBehaviour
    {
        public float duration;
        public float elapsed;
        public Ease ease;
        public bool active;

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
                SetComponentEnabled<EaseComponent>(entity, authoring.active);
            }
        }
    }
}