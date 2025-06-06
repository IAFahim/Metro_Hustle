using System.Collections.Generic;
using _src.Scripts.Missions.Missions.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _src.Scripts.UiServices.Missions.Service
{
    public class MissionsService : IMissionsService
    {
        private readonly List<AssetReferenceMissions> assets;

        public MissionsService()
        {
            this.assets = Object.FindAnyObjectByType<MissionsServiceBehaviour>()?.assets;
        }

        public AssetReferenceMissions GetAsset(ushort index)
        {
            return this.assets[index];
        }

        public AssetReferenceMissions GetCurrent()
        {
            var currentLocation = PlayerPrefs.GetInt("Location", 0);
            return assets[currentLocation];
        }
    }
}