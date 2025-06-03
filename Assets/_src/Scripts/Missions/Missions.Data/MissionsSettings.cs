using System;
using System.Collections.Generic;
using BovineLabs.Core.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _src.Scripts.Missions.Missions.Data
{
    [CreateAssetMenu(fileName = "AllMissions", menuName = "Scriptable Objects/Mission/Settings", order = 1)]
    public class MissionsSettings : ScriptableObject
    {
        public List<AssetReferenceMissionSchema> missionSchema = new();
        
    }
    
    [Serializable]
    public class AssetReferenceMissionSchema: AssetReferenceT<MissionSchema>
    {
        public AssetReferenceMissionSchema(string guid) : base(guid)
        {
        }
    }
}