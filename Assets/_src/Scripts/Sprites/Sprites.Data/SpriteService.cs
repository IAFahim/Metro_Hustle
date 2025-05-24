using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _src.Scripts.Sprites.Sprites.Data
{
    [UsedImplicitly]
    public class SpriteService: ISpriteService
    {
        private readonly Dictionary<string, Sprite> assets;
        
        public SpriteService()
        {
            var uxmls = Object.FindAnyObjectByType<SpritesServiceBehaviour>()?.Values ?? Array.Empty<SpritesServiceBehaviour.KeySprites>();
            this.assets = uxmls.ToDictionary(t => t.Key, t => t.Asset);
        }
        
        public Sprite GetAsset(string assetName)
        {
            try
            {
                return this.assets[assetName];
            }
            catch (KeyNotFoundException)
            {
                Debug.LogError($"Can't find key {assetName}");
                throw;
            }
        }
    }
}