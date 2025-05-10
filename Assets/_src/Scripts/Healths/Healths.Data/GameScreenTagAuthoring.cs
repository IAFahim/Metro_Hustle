using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Healths.Healths.Data
{
    public class GameScreenTagAuthoring : MonoBehaviour
    {
        public class GameScreenTagBaker : Baker<GameScreenTagAuthoring>
        {
            public override void Bake(GameScreenTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<GameScreenTag>(entity);
            }
        }
    }
}