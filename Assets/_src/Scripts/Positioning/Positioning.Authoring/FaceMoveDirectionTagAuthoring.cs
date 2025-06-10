using _src.Scripts.Positioning.Positioning.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Positioning.Positioning.Authoring
{
    internal class FaceMoveDirectionTagAuthoring : MonoBehaviour
    {
        internal class FaceMoveDirectionComponentBaker : Baker<FaceMoveDirectionTagAuthoring>
        {
            public override void Bake(FaceMoveDirectionTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new FaceMoveDirectionTag());
            }
        }
    }
}