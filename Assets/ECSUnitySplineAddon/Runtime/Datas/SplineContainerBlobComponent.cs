using Unity.Entities;

namespace ECSSplines.Runtime
{
    /// <summary>
    /// ECS Component holding a reference to the baked NativeSplineContainerBlob asset.
    /// Represents an entire SplineContainer. Added to an entity during baking.
    /// </summary>
    public struct SplineContainerBlobComponent : IComponentData
    {
        public BlobAssetReference<NativeSplineContainerBlob> Value;
    }
}