using System;
using _src.Scripts.StatsHelpers.StatsHelpers.Data;
using BovineLabs.Core.ObjectManagement;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _src.Scripts.Missions.Missions.Data
{
    [CreateAssetMenu(fileName = "MissionSchema", menuName = "Scriptable Objects/Mission/Schema")]
    public class MissionSchema : ScriptableObject, IUID
    {
        [Header("Identification & Display")] public int id;
        public string title = "New Mission";
        [TextArea(3, 5)] public string description;
        public AssetReferenceSprite spriteAsset;

        [Header("Route & Parcel")] public string startStation;
        public string endStation;
        public DayTime dayTime;

        [Header("Objectives")] public Objective[] objectives;

        public int ID
        {
            get => id;
            set => id = value;
        }
    }


    [Serializable]
    public struct Objective
    {
        public bool optional;
        public string name;
        public AssetReferenceSprite spriteAsset;
        public Item[] reward;
        public Goal goal;
    }

    [Serializable]
    public struct Item
    {
        public EIntrinsic intrinsic;
        public ushort count;
    }

    [Serializable]
    public struct Goal
    {
        public EIntrinsic intrinsic;
        public ObjectiveComparison comparison;
        public ushort defaultValue;
        public ushort min;
        public ushort max;
        public half timeLimit; 
    }

    public enum DayTime : byte
    {
        Morning = 0,
        Day = 1,
        Evening = 2,
        Night = 3,
        LateNight = 4,
    }

    public enum ParcelType : byte
    {
        Standard,
        Lightweight, // Might grant a temporary EStat.BaseMoveSpeed boost
        Heavy, // Might inflict a temporary EStat.BaseMoveSpeed reduction
        Fragile // Mission fails on any collision
    }

    public enum ObjectiveType : byte
    {
        DeliverPackage, // Implicitly the main goal: reach end station
        CollectRunIntrinsic, // e.g., EIntrinsic.TrackCashCollectedThisRun >= X
        ReachRunDistance, // e.g., EIntrinsic.CurrentRunDistance >= X
        PerformActionCount, // e.g., EIntrinsic.JumpsPerformedThisRun >= X
        AvoidCollisions, // e.g., EIntrinsic.ObstaclesCollidedThisRun == 0
        StayBelowCollisionCount, // e.g., EIntrinsic.ObstaclesCollidedThisRun <= X
        BeatRunTimer, // e.g., EIntrinsic.RunTimer <= X
        MaintainParkourFlow, // e.g., EIntrinsic.ParkourFlowPointsThisRun >= X
        // Rival related objectives might need more specific handling or sub-types
    }

    public enum ObjectiveComparison : byte
    {
        GreaterThanOrEqual,
        LessThanOrEqual,
        EqualTo,
        NotEqualTo // Less common, but possible
    }
}