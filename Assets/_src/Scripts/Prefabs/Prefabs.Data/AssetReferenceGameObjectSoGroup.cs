using UnityEngine;

namespace _src.Scripts.Prefabs.Prefabs.Data
{
    [CreateAssetMenu(menuName = "Game/AssetReference/GameObjectSoGroup")]
    public class AssetReferenceGameObjectSoGroup : ScriptableObject
    {
        public AssetReferenceGameObjectSo[] assets;
    }
}