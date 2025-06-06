using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _src.Scripts.Missions.Missions.Data
{
    [CreateAssetMenu(fileName = "AllMissions", menuName = "Scriptable Objects/Mission/Settings", order = 1)]
    public class MissionsSettings : ScriptableObject
    {
        public List<MissionSchema> missionSchema = new();
        private static bool _isActive;
        public static bool IsActive => _isActive;
        public static MissionSchema CurrentMission;

        private void OnEnable()
        {
            _isActive = true;
        }
    }
    
    [Serializable]
    public class AssetReferenceMissions: AssetReferenceT<MissionsSettings>
    {
        public AssetReferenceMissions(string guid) : base(guid)
        {
        }
    }
}