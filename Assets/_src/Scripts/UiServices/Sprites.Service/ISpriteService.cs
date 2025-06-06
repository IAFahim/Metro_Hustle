using UnityEngine;

namespace _src.Scripts.UiServices.Sprites.Service
{
    public interface ISpriteService
    {
        Sprite GetAsset(ushort id);
    }
}