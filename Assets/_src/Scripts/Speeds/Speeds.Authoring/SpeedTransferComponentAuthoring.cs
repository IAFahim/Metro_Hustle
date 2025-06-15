using _src.Scripts.Speeds.Speeds.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Speeds.Speeds.Authoring
{
    public class SpeedTransferComponentAuthoring : MonoBehaviour
    {
        public half speed;

        public class SpeedTransferComponentBaker : Baker<SpeedTransferComponentAuthoring>
        {
            public override void Bake(SpeedTransferComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new SpeedTransferComponent { Speed = authoring.speed });
            }
        }
    }
}