using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.Direction;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls.Authoring
{
    public class DirectionInputAuthoring : MonoBehaviour
    {
        public bool enable;
        [Header("ComponentEnable")]
        public bool rightInputEnable;
        public bool leftInputEnable;
        
        [Header("LiveInput")]
        public bool rightInputLive;
        public bool leftInputLive;


        private class DirectionInputBaker : Baker<DirectionInputAuthoring>
        {
            public override void Bake(DirectionInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<PlayerInputEnableTag>(entity);
                SetComponentEnabled<PlayerInputEnableTag>(entity, authoring.enable);

                SetRight(authoring, entity);
                SetLeft(authoring, entity);
            }

            private void SetLeft(DirectionInputAuthoring authoring, Entity entity)
            {
                AddComponent(entity, new LeftInputEnabledTag
                {
                    Live = authoring.leftInputLive
                });
                SetComponentEnabled<LeftInputEnabledTag>(entity, authoring.leftInputEnable);
            }

            private void SetRight(DirectionInputAuthoring authoring, Entity entity)
            {
                AddComponent(entity, new RightInputEnabledTag
                {
                    Live = authoring.rightInputLive
                });
                SetComponentEnabled<RightInputEnabledTag>(entity, authoring.rightInputEnable);
            }
        }
    }
}