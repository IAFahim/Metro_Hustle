using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using Unity.Entities;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data
{
    public struct SideEffectBlobData : IComponentData
    {
        public BlobAssetReference<KvIntrinsic> Value;
        
    }
    public struct SideEffectBlob
    {
        public BlobArray<KvIntrinsic> KvIntrinsics;
    }
}