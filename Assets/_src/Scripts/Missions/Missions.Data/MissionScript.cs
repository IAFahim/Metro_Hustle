using System;
using BovineLabs.Stats.Data;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.Missions.Missions.Data
{
    [CreateAssetMenu(fileName = "MissionScript", menuName = "Scriptable Objects/MissionScript")]
    public class MissionScript : ScriptableObject
    {
        public int number;
        public string title;
        public Objective[] objectives;
        public Station startingStation;
        public Station endEndStation;
        public DayTime dayTime;
        public float distance;
        public Item[] items;
        public Item[] reward;
    }

[Serializable]
    public class Station
    {
        public string name;
        public string shortName;
        public byte splineIndex;
        public byte curveIndex;
        public half curveDistance;
        
    }

    [Serializable]
    public class Objective
    {
        public string name;
        public ushort stats;
        public Goal goal;
    }
    
    [Serializable]
    public class Item
    {
        public string name;
        public IntrinsicKey intrinsicKey;
        public Goal goal;
    }

    [Serializable]
    public class Goal
    {
        public ushort defaultValue;
        public ushort min;
        public ushort max;
    }
    
    public enum DayTime : byte
    {
        Morning = 0,
        Day = 1,
        Evening = 2,
        Night = 3,
        LateNight = 4,
    }
}