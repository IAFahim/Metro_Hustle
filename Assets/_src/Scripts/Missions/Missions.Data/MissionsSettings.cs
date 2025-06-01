using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _src.Scripts.Missions.Missions.Data
{
    [CreateAssetMenu(fileName = "AllMissions", menuName = "Metro Hustle/All Missions Registry", order = 1)]
    public class MissionsSettings : ScriptableObject
    {
        public List<MissionSchema> missionSchema = new();

        private Dictionary<string, MissionSchema> _missionLookup;

        public MissionSchema GetMissionByID(string id)
        {
            if (_missionLookup == null || _missionLookup.Count != missionSchema.Count)
            {
                _missionLookup = missionSchema.Where(m => m && !string.IsNullOrEmpty(m.guid4))
                    .ToDictionary(m => m.guid4, m => m);
            }
            _missionLookup.TryGetValue(id, out MissionSchema mission);
            return mission;
        }

#if UNITY_EDITOR
        // Helper to find all MissionDataSO assets and populate the list
        [ContextMenu("Find All Mission Assets")]
        void FindAllMissionAssets()
        {
            missionSchema.Clear();
            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(MissionSchema)}");
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var mission = UnityEditor.AssetDatabase.LoadAssetAtPath<MissionSchema>(path);
                if (mission && !missionSchema.Contains(mission))
                {
                    missionSchema.Add(mission);
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log($"Found and added {missionSchema.Count} missions to the registry.");
        }
#endif
    }
}