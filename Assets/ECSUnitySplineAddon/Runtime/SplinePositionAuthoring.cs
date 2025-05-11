using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECSSplines.Runtime
{
    /// <summary>
    /// Authoring component to define an entity's starting position and behaviour
    /// relative to a spline within a SplineContainer.
    /// </summary>
    [DisallowMultipleComponent]
    public class SplinePositionAuthoring : MonoBehaviour
    {
        [Header("Spline Reference")] [Tooltip("The zero-based index of the spline within the target container to follow.")]
        public int TargetSplineIndex = 0;

        [Header("Initial State")]
        [Tooltip("Starting progress along the spline, normalized (0 = start, 1 = end).")]
        [Range(0f, 1f)]
        public float InitialNormalizedTime = 0f;

        [Tooltip("Initial movement speed along the spline (units/second).")]
        public float Speed = 5f;

        [Tooltip("Initial offset from the spline path (X=Right, Y=Up, Z=Forward relative to spline tangent/up).")]
        public Vector3 Offset = Vector3.zero;


        private class SplinePositionBaker : Baker<SplinePositionAuthoring>
        {
            public override void Bake(SplinePositionAuthoring authoring)
            {
                var movingEntity = GetEntity(TransformUsageFlags.Dynamic);


                AddComponent(movingEntity, new SplinePositionData
                {
                    TargetSplineIndex = authoring.TargetSplineIndex,
                    NormalizedTime = authoring.InitialNormalizedTime,
                    Speed = authoring.Speed,
                    Offset = (float3)authoring.Offset
                });
            }
        }
    }
}