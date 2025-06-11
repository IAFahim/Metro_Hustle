using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace _src.Scripts.InputControls.InputControls.Authoring
{
    [RequireComponent(typeof(InputEnableTagAuthoring))]
    public class DirectionInputAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("directionEnableActiveFlag")] public InputDirectionFlag inputDirectionFlag = InputDirectionFlag.EnableFlagsMask;

        private class DirectionInputBaker : Baker<DirectionInputAuthoring>
        {
            public override void Bake(DirectionInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new DirectionInputEnableActiveComponent
                {
                    Flag = authoring.inputDirectionFlag
                });
            }
        }
    }
}