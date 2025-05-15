using _src.Scripts.InputControls.InputControls.Data.Direction;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls.Authoring
{
    [RequireComponent(typeof(InputEnableTagAuthoring))]
    public class DirectionInputAuthoring : MonoBehaviour
    {
        public DirectionFlag directionFlag = DirectionFlag.EnableFlagsMask;

        private class DirectionInputBaker : Baker<DirectionInputAuthoring>
        {
            public override void Bake(DirectionInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DirectionInputComponent
                {
                    DirectionFlag = authoring.directionFlag
                });
            }
        }
    }
}