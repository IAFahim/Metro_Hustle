using _src.Scripts.SplineMovements.SplineMovements.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.SplineMovements.SplineMovements.Authoring
{
    public class SplineLineComponentAuthoring : MonoBehaviour
    {
        [Range(0, 31)] public byte spline;
        [Range(0, 7)] public byte line = 1;

        public class SplineLineComponentBaker : Baker<SplineLineComponentAuthoring>
        {
            public override void Bake(SplineLineComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, SplineLineComponent.Create(authoring.spline, authoring.line));
            }
        }
    }
}