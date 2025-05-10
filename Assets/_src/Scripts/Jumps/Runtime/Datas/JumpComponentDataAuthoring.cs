#if UNITY_EDITOR
using _src.Scripts.Easings.Runtime.Datas;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Jumps.Runtime.Datas
{
    public class JumpComponentDataAuthoring : MonoBehaviour
    {
        public float maxHeight = 2.0f;
        
        [Header("Rise")] public Ease riseEase = Ease.OutSine;
        public float riseDuration = 1.0f;
        public float riseDurationMultiplier = 1f;
        
        [Header("Air")] public float airTime = 0;
        
        [Header("Fall")] public Ease fallEase = Ease.InSine;
        public float fallDuration = 1.0f;
        public float fallDurationMultiplier = 1;


        public class JumpComponentDataBaker : Baker<JumpComponentDataAuthoring>
        {
            public override void Bake(JumpComponentDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var height = authoring.transform.position.y;
                AddComponent(entity, new JumpComponentData
                    {
                        CurrentHeight = height,
                        RiseEase = authoring.riseEase,
                        FallEase = authoring.fallEase,
                        RiseDuration = authoring.riseDuration,
                        FallDuration = authoring.fallDuration,
                        MaxHeight = authoring.maxHeight,
                        AirTime = authoring.airTime,
                        FallDurationMultiplier = authoring.fallDurationMultiplier,
                        RiseDurationMultiplier = authoring.riseDurationMultiplier,
                    }
                );
            }
        }
    }
}
#endif