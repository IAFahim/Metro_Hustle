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


        public (MissionSchema mission, int missionNumber) GetCurrent()
        {
            var missionNumber = PlayerPrefs.GetInt("current_mission", 0);
            return (missionSchema[missionNumber], missionNumber + 1);
        }
    }

    [Serializable]
    public class AssetReferenceMissions : AssetReferenceT<MissionsSettings>
    {
        public AssetReferenceMissions(string guid) : base(guid)
        {
        }
    }
}