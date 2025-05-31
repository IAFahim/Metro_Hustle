using _src.Scripts.Gaps.Gaps.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Gaps.Gaps.Authoring
{
    internal class GapZComponentAuthoring : MonoBehaviour
    {
        public half forward = new(1);
        public half backword = new(1);

        private class BuildingGapComponentBaker : Baker<GapZComponentAuthoring>
        {
            public override void Bake(GapZComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GapZComponent
                {
                    Forward = authoring.forward, Backward = authoring.backword
                });
            }
        }
    }
}