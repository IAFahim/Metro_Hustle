using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace _src.Scripts.UXMLs.UXMLs.Data
{
    public class UxmlServiceBehaviour : MonoBehaviour
    {
        [SerializeField]
        private KeyUXML[] values = Array.Empty<KeyUXML>();

        public KeyUXML[] Values => this.values;

        [Serializable]
        public class KeyUXML
        {
            public string Key;
            public VisualTreeAsset Asset;
        }
    }
}