using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using BovineLabs.Core.ObjectManagement;
using UnityEngine;

namespace _src.Scripts.TriggerSideEffects.TriggerSideEffects.Authoring
{
    [CreateAssetMenu(fileName = "triggerSideEffectConfig", menuName = "Scriptable Objects/SideEffect/Schema",
        order = 0)]
    [AutoRef(nameof(SideEffectSettings), nameof(SideEffectSchemaObject), nameof(SideEffectSchemaObject),
        "Schemas/SideEffect/")]
    public class SideEffectSchemaObject : ScriptableObject
    {
        public KvIntrinsic[] sideEffect;
    }
}