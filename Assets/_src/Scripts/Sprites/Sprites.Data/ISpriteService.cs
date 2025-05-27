using UnityEngine;

namespace _src.Scripts.Sprites.Sprites.Data
{
    public interface ISpriteService
    {
        Sprite GetAsset(ushort id);
    }
}