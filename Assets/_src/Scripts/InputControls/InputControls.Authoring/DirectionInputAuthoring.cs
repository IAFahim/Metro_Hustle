using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.Direction;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls.Authoring
{
    public class DirectionInputAuthoring : MonoBehaviour
    {
        public bool playerInputEnable;
        
        [Header("ComponentEnable")] public bool upInputEnable = true;
        public bool downInputEnable = true;
        public bool rightInputEnable = true;
        public bool leftInputEnable = true;

        [Header("LiveInput")] public bool upInputLive;
        public bool downInputLive;
        public bool rightInputLive;
        public bool leftInputLive;


        private class DirectionInputBaker : Baker<DirectionInputAuthoring>
        {
            public override void Bake(DirectionInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<PlayerInputEnableTag>(entity);
                SetComponentEnabled<PlayerInputEnableTag>(entity, authoring.playerInputEnable);

                SetUp(authoring, entity);
                SetDown(authoring, entity);
                SetRight(authoring, entity);
                SetLeft(authoring, entity);
            }

            private void SetUp(DirectionInputAuthoring authoring, Entity entity)
            {
                AddComponent(entity, new UpInputEnabledTag
                {
                    Live = authoring.upInputLive
                });
                SetComponentEnabled<UpInputEnabledTag>(entity, authoring.upInputEnable);
            }

            private void SetDown(DirectionInputAuthoring authoring, Entity entity)
            {
                AddComponent(entity, new DownInputEnabledTag
                {
                    Live = authoring.downInputLive
                });
                SetComponentEnabled<DownInputEnabledTag>(entity, authoring.downInputEnable);
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