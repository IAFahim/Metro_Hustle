using System;
using _src.Scripts.ZBuildings.ZBuildings.Data;
using BovineLabs.Core.PropertyDrawers;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Authoring
{
    internal class BuildingGroupsAuthoring : MonoBehaviour
    {
        [SerializeField] private BuildingGroup[] leftGroups;
        [SerializeField] private BuildingGroup[] rightGroups;

        internal class BuildingGroupLeftBaker : Baker<BuildingGroupsAuthoring>
        {
            public override void Bake(BuildingGroupsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                var buildingGroupLefts = AddBuffer<BuildingGroupLeft>(entity);
                foreach (var group in authoring.leftGroups)
                {
                    buildingGroupLefts.Add(new BuildingGroupLeft()
                    {
                        StartOffset = group.startOffset,
                        Prefab = GetEntity(group.prefab, TransformUsageFlags.Renderable),
                    });
                }
                
                var buildingGroupRights = AddBuffer<BuildingGroupRight>(entity);
                foreach (var group in authoring.leftGroups)
                {
                    buildingGroupRights.Add(new BuildingGroupRight()
                    {
                        StartOffset = group.startOffset,
                        Prefab = GetEntity(group.prefab, TransformUsageFlags.Renderable),
                    });
                }
            }
        }
        
        [Serializable]
        internal struct BuildingGroup
        {
            public half startOffset;
            [PrefabElement] public GameObject prefab;
        }
    }
}