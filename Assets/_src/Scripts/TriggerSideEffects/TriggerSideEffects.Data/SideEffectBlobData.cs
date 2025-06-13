using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using Unity.Entities;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data
{
    public struct SideEffectBlobData : IComponentData
    {
        public BlobAssetReference<BlobArray<BlobArray<KvIntrinsic>>> SideEffectsArray;
    }
}