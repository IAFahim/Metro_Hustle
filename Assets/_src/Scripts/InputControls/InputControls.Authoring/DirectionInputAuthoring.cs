using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.enums;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls.Authoring
{
    [RequireComponent(typeof(InputEnableTagAuthoring))]
    public class DirectionInputAuthoring : MonoBehaviour
    {
        public DirectionEnableActiveFlag directionEnableActiveFlag = DirectionEnableActiveFlag.EnableFlagsMask;
        public bool addLock;
        public DirectionLockFlag directionLockFlag;

        private class DirectionInputBaker : Baker<DirectionInputAuthoring>
        {
            public override void Bake(DirectionInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new DirectionInputEnableActiveComponent
                {
                    Flag = authoring.directionEnableActiveFlag
                });

                if (!authoring.addLock) return;
                AddComponent(entity, new DirectionLockComponent
                {
                    Flag = authoring.directionLockFlag
                });
            }
        }
    }
}