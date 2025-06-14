using _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Authoring
{
    public class SideEffectLinkComponentAuthoring : MonoBehaviour
    {
        public SideEffectSchemaObject sideEffectSchemaObject;

        public class SideEffectLinkComponentBaker : Baker<SideEffectLinkComponentAuthoring>
        {
            public override void Bake(SideEffectLinkComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SideEffectLinkComponent
                {
                    Index = (byte)authoring.sideEffectSchemaObject.ID
                });
            }
        }
    }
}