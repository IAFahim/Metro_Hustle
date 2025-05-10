using Unity.Entities;
using Unity.Mathematics;

namespace ECSSplines.Runtime
{
    /// <summary>
    /// Component to track an entity's position and state relative to a specific spline
    /// within a baked NativeSplineContainerBlob.
    /// </summary>
    public struct SplinePositionData : IComponentData
    {
        public int TargetSplineIndex;

        public float NormalizedTime;

        public float Speed;

        public float3 Offset;
    }
}