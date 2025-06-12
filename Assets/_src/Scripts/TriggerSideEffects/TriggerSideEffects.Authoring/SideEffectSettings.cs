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
        public SideEffectSchemaObject[] triggerSideEffect = Array.Empty<SideEffectSchemaObject>();

        public override void Bake(Baker<SettingsAuthoring> baker)
        {
            var entity = baker.GetEntity(TransformUsageFlags.None);
            SetupSideEffect(baker, entity);
        }

        private void SetupSideEffect(Baker<SettingsAuthoring> baker, Entity entity)
        {
            var builder = new BlobBuilder(Allocator.Temp);
            ref var blobData = ref builder.ConstructRoot<SideEffectBlob>();
            var blobReference = builder.CreateBlobAssetReference<KvIntrinsic>(Allocator.Persistent);
            builder.Dispose();
            baker.AddBlobAsset(ref blobReference, out var hash);
            baker.AddComponent(entity, new SideEffectBlobData
            {
                Value = blobReference
            });
        }
    }
}