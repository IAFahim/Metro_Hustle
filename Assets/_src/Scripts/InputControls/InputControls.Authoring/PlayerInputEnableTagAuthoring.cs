using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.InputControls.InputControls.Data
{
    public class PlayerInputEnableTagAuthoring : MonoBehaviour
    {
        public bool playerInputEnable;
        public class PlayerInputEnableTagBaker : Baker<PlayerInputEnableTagAuthoring>
        {
            public override void Bake(PlayerInputEnableTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlayerInputEnableTag>(entity);
                SetComponentEnabled<PlayerInputEnableTag>(entity, authoring.playerInputEnable);

            }
        }
    }
}