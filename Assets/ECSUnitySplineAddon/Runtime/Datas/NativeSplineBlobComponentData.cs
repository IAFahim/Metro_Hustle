using Unity.Entities;

namespace ECSUnitySplineAddon.Runtime.Datas
{
    public struct NativeSplineBlobComponentData : IComponentData
    {
        public BlobAssetReference<NativeSplineBlob> Value;
    }
}