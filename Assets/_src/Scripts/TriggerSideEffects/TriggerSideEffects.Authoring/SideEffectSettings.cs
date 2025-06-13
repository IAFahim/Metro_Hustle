using System;
using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data;
using BovineLabs.Core.Authoring.Settings;
using BovineLabs.Core.Settings;
using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Authoring
{
    [SettingsGroup(nameof(SideEffectSettings))]
    public class SideEffectSettings : SettingsBase
    {
        public SideEffectSchemaObject[] sideEffectSchemas = Array.Empty<SideEffectSchemaObject>();

        public override void Bake(Baker<SettingsAuthoring> baker)
        {
            var entity = baker.GetEntity(TransformUsageFlags.None);
            SetupSideEffect(baker, entity);
        }

        private void SetupSideEffect(Baker<SettingsAuthoring> baker, Entity entity)
        {
            var sideEffectsArray = SideEffectBlobRef();
            baker.AddBlobAsset(ref sideEffectsArray, out _);
            baker.AddComponent(entity, new SideEffectBlobData
            {
                SideEffectsArray = sideEffectsArray
            });
        }

        private BlobAssetReference<BlobArray<BlobArray<KvIntrinsic>>> SideEffectBlobRef()
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<BlobArray<BlobArray<KvIntrinsic>>>();
            
            var arrayBuilder = builder.Allocate(ref root, sideEffectSchemas.Length);
            for (var i = 0; i < sideEffectSchemas.Length; i++)
            {
                var kvIntrinsics = sideEffectSchemas[i].sideEffects;
                var sideEffectsLength = kvIntrinsics.Length;
                var subArrayBuilder = builder.Allocate(ref arrayBuilder[i], sideEffectsLength);
                for (int j = 0; j < sideEffectsLength; j++) subArrayBuilder[j] = kvIntrinsics[j];
            }

            return builder.CreateBlobAssetReference<BlobArray<BlobArray<KvIntrinsic>>>(Allocator.Persistent);
        }
    }
}