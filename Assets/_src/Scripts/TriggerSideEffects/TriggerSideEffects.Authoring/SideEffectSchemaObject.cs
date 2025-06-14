using System;
using _src.Scripts.StatsHelpers.StatsHelpers.Authoring;
using BovineLabs.Core.ObjectManagement;
using BovineLabs.Core.PropertyDrawers;
using UnityEngine;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Authoring
{
    [AutoRef(
        nameof(SideEffectSettings),
        nameof(SideEffectSchemaObject),
        nameof(SideEffectSchemaObject),
        "Schemas/SideEffect/")
    ]
    public class SideEffectSchemaObject : ScriptableObject, IUID
    {
        public KvSchemaIntrinsic[] sideEffects = Array.Empty<KvSchemaIntrinsic>();
        [InspectorReadOnly] [SerializeField] private byte key;

        /// <inheritdoc/>
        public int ID
        {
            get => this.key;
            set => this.key = (byte)value;
        }
    }
}