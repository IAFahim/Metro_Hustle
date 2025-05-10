using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace _src.Scripts.Animations.Animations.Data
{
    public class AnimatorPrefabIndexComponentAuthoring : MonoBehaviour
    {
        // negative to load main player
        public sbyte index = -1;

        public class AnimatorPrefabIndexComponentBaker : Baker<AnimatorPrefabIndexComponentAuthoring>
        {
            public override void Bake(AnimatorPrefabIndexComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new AnimatorPrefabIndexComponent { Index = authoring.index });
            }
        }
    }
}