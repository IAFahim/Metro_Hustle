using _src.Scripts.TriggerSideEffects.TriggerSideEffects.Data;
using BovineLabs.Core.Authoring.ObjectManagement;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Authoring
{
    internal class TriggerSideEffectSpawnComponentAuthoring : MonoBehaviour
    {
        [Header("Auto-calculated based on assigned ObjectDefinitions")] [SerializeField]
        private TriggerType triggerType;

        [Header("Object Definitions")] [CanBeNull]
        public ObjectDefinition onForwardPre;

        [CanBeNull] public ObjectDefinition onTop;
        [CanBeNull] public ObjectDefinition onInside;

        public TriggerType TriggerType => triggerType;

        private void OnValidate()
        {
            UpdateTriggerType();
        }

        private void UpdateTriggerType()
        {
            TriggerType newTriggerType = TriggerType.Nothing;

            
            if (onForwardPre != null) newTriggerType |= TriggerType.HasForward;
            if (onTop != null) newTriggerType |= TriggerType.HasTop;
            if (onInside != null) newTriggerType |= TriggerType.HasInside;

            if (triggerType != newTriggerType)
            {
                triggerType |= newTriggerType;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }


        private class TriggerSideEffectComponentSpawnBaker : Baker<TriggerSideEffectSpawnComponentAuthoring>
        {
            public override void Bake(TriggerSideEffectSpawnComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                DependsOn(authoring.onForwardPre);
                DependsOn(authoring.onTop);
                DependsOn(authoring.onInside);
                
                AddComponent(entity, new TriggerSideEffectSpawnComponent
                {
                    TriggerType = authoring.TriggerType,
                    OnForwardPre = authoring.onForwardPre,
                    OnTop = authoring.onTop,
                    OnInside = authoring.onInside
                });
            }
        }
    }
}