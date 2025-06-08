using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _src.Scripts.Missions.Missions.Data
{
    [CreateAssetMenu(menuName = "Item", fileName = "Scriptable Objects/Mission/Item")]
    public class Item : ScriptableObject
    {
        public string title;
        public AssetReferenceSprite spriteAsset;
        public half weight;
    }

    [Serializable]
    public class AssetReferenceItem : AssetReferenceT<Item>
    {
        public AssetReferenceItem(string guid) : base(guid)
        {
        }
    }
}