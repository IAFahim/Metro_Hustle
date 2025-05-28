using _src.Scripts.ZMovements.ZMovements.Data;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.ZMovements.ZMovements.Authoring
{
    public class ZProgressComponentAuthoring : MonoBehaviour
    {
        public half Progress;

        public class ZProgressComponentBaker : Baker<ZProgressComponentAuthoring>
        {
            public override void Bake(ZProgressComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new ZProgressComponent { Progress = authoring.Progress });
            }
        }
    }
}