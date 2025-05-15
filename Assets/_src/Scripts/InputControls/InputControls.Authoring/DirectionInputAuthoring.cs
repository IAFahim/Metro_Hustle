using _src.Scripts.InputControls.InputControls.Data;
using _src.Scripts.InputControls.InputControls.Data.Direction;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls.Authoring
{
    public class DirectionInputAuthoring : MonoBehaviour
    {
        public bool inputEnabled = true;
        public bool playerInputEnable;
        
        [Header("ComponentEnable")] public bool upInputEnable = true;
        public bool downInputEnable = true;
        public bool sideMoveInputEnable = true;

        [Header("LiveInput")] 
        public sbyte speedLevel;
        public bool sideInputLive;
        public bool isRight;


        private class DirectionInputBaker : Baker<DirectionInputAuthoring>
        {
            public override void Bake(DirectionInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<InputEnabledTag>(entity);
                SetComponentEnabled<InputEnabledTag>(entity, authoring.inputEnabled);
                AddComponent<PlayerInputEnableTag>(entity);
                SetComponentEnabled<PlayerInputEnableTag>(entity, authoring.playerInputEnable);

                SetSpeed(authoring, entity);
                SetSideMove(authoring, entity);
            }

            private void SetSpeed(DirectionInputAuthoring authoring, Entity entity)
            {
                AddComponent(entity, new SpeedValueEnabledTag
                {
                   Level = authoring.speedLevel
                });
                SetComponentEnabled<SpeedValueEnabledTag>(entity, authoring.upInputEnable);
            }
            
            private void SetSideMove(DirectionInputAuthoring authoring, Entity entity)
            {
                AddComponent(entity, new SideMoveInputEnableTag
                {
                    Live = authoring.sideInputLive,
                    IsRight = authoring.isRight
                });
                SetComponentEnabled<SideMoveInputEnableTag>(entity, authoring.sideMoveInputEnable);
            }
        }
    }
}