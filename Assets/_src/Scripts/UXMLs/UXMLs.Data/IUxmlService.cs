using UnityEngine.UIElements;

namespace _src.Scripts.UXMLs.UXMLs.Data
{
    public interface IUxmlService
    {
        VisualTreeAsset GetAsset(string assetName);
    }
}