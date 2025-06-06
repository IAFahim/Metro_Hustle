using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _src.Scripts.UiServices.Sprites.Service
{
    [UsedImplicitly]
    public class SpriteService: ISpriteService
    {
        private readonly Dictionary<ushort, Sprite> assets;
        
        public SpriteService()
        {
            var uxmls = Object.FindAnyObjectByType<SpritesServiceBehaviour>()?.Values ?? Array.Empty<SpritesServiceBehaviour.KeySprites>();
            this.assets = uxmls.ToDictionary(t => t.Key, t => t.Asset);
        }
        
        public Sprite GetAsset(ushort assetName)
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