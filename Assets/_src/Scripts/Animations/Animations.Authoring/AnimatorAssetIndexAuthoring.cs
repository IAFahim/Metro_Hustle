using _src.Scripts.Animations.Animations.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Animations.Animations.Authoring
{
    public class AnimatorAssetIndexAuthoring : MonoBehaviour
    {
        public sbyte index = -1;

        private class AnimatorAssetIndexAuthoringBaker : Baker<AnimatorAssetIndexAuthoring>
        {
            public override void Bake(AnimatorAssetIndexAuthoring assetIndexAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new AnimatorAssetIndexComponent()
                {
                    Index = assetIndexAuthoring.index
                });
            }
        }
    }
}