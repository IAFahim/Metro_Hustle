using System;
using UnityEngine;

namespace _src.Scripts.Sprites.Sprites.Data
{
    public class SpritesServiceBehaviour : MonoBehaviour
    {
        [SerializeField] private KeySprites[] values = Array.Empty<KeySprites>();

        public KeySprites[] Values => this.values;

        [Serializable]
        public class KeySprites
        {
            public ushort Key;
            public Sprite Asset;
        }
    }
}