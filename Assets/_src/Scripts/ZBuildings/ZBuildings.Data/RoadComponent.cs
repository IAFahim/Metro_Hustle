using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace _src.Scripts.ZBuildings.ZBuildings.Data
{
    [BurstCompile]
    public struct RoadComponent : IComponentData
    {
        public half SizeZ;
        public half SideGap;
        public half PerLineWidth;
        public RoadFlag RoadFlag;

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasFlagFast(RoadFlag flag) => (RoadFlag & flag) == flag;

        [BurstCompile]
        public readonly int GetRoadCount()
        {
            byte value = (byte)RoadFlag;
            int count = 0;
            while (value != 0)
            {
                count += value & 1;
                value >>= 1;
            }

            return count;
        }

        [BurstCompile]
        public readonly bool TryGetAdjacentPosition(RoadFlag singleActiveBitCurrent, bool goRight, out float position)
        {
            position = 0f;
            int currentFlagValue = (int)singleActiveBitCurrent;
            int existingRoadFlags = (int)RoadFlag;

            int nextPotentialLineFlagValue;

            if (goRight)
            {
                if (singleActiveBitCurrent is RoadFlag.Right3 or RoadFlag.None) return false;
                nextPotentialLineFlagValue = currentFlagValue >> 1;
            }
            else
            {
                if (singleActiveBitCurrent is RoadFlag.Left3 or RoadFlag.None) return false;
                nextPotentialLineFlagValue = currentFlagValue << 1;
            }

            int nextLineSpatialIndex = GetFlagPosition((RoadFlag)nextPotentialLineFlagValue);
            if ((existingRoadFlags & nextPotentialLineFlagValue) != nextPotentialLineFlagValue) return false;
            position = nextLineSpatialIndex * PerLineWidth;
            return true;
        }

        [BurstCompile]
        public readonly float GetTotalWidth(bool withGap)
        {
            int roadCount = GetRoadCount();
            if (roadCount == 0) return 0f;

            float totalLineWidth = roadCount * (float)PerLineWidth;
            if (!withGap) return totalLineWidth;
            float sideGaps = SideGap * 2f;
            return totalLineWidth + sideGaps;
        }

        [BurstCompile]
        public readonly float3 CalculateExtern(float roadTriggerHeight, float sizeZ,bool withGap)
        {
            var totalWidth = GetTotalWidth(withGap);
            return new float3(totalWidth / 2, roadTriggerHeight, sizeZ / 2);
        }


        [BurstCompile]
        public readonly bool HasAnyLeftRoad()
        {
            return HasFlagFast(RoadFlag.Left1) || HasFlagFast(RoadFlag.Left2) || HasFlagFast(RoadFlag.Left3);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool HasAnyRightRoad()
        {
            return HasFlagFast(RoadFlag.Right1) || HasFlagFast(RoadFlag.Right2) || HasFlagFast(RoadFlag.Right3);
        }

        [BurstCompile]
        public readonly RoadFlag GetLeftmostRoad()
        {
            if (HasFlagFast(RoadFlag.Left3)) return RoadFlag.Left3;
            if (HasFlagFast(RoadFlag.Left2)) return RoadFlag.Left2;
            if (HasFlagFast(RoadFlag.Left1)) return RoadFlag.Left1;
            if (HasFlagFast(RoadFlag.Center)) return RoadFlag.Center;
            if (HasFlagFast(RoadFlag.Right1)) return RoadFlag.Right1;
            if (HasFlagFast(RoadFlag.Right2)) return RoadFlag.Right2;
            if (HasFlagFast(RoadFlag.Right3)) return RoadFlag.Right3;
            return RoadFlag.None;
        }

        [BurstCompile]
        public readonly RoadFlag GetRightmostRoad()
        {
            if (HasFlagFast(RoadFlag.Right3)) return RoadFlag.Right3;
            if (HasFlagFast(RoadFlag.Right2)) return RoadFlag.Right2;
            if (HasFlagFast(RoadFlag.Right1)) return RoadFlag.Right1;
            if (HasFlagFast(RoadFlag.Center)) return RoadFlag.Center;
            if (HasFlagFast(RoadFlag.Left1)) return RoadFlag.Left1;
            if (HasFlagFast(RoadFlag.Left2)) return RoadFlag.Left2;
            if (HasFlagFast(RoadFlag.Left3)) return RoadFlag.Left3;
            return RoadFlag.None;
        }


        [BurstCompile]
        public readonly int GetFlagPosition(RoadFlag flag)
        {
            // Returns spatial position from center (negative = left, positive = right)
            return flag switch
            {
                RoadFlag.Left3 => -3,
                RoadFlag.Left2 => -2,
                RoadFlag.Left1 => -1,
                RoadFlag.Center => 0,
                RoadFlag.Right1 => 1,
                RoadFlag.Right2 => 2,
                RoadFlag.Right3 => 3,
                _ => int.MaxValue // Invalid flag
            };
        }
    }

    // Zero means disable
    [Flags]
    public enum RoadFlag : sbyte
    {
        None = 0,
        Left3 = 0b0100_0000, // 64
        Left2 = 0b0010_0000, // 32
        Left1 = 0b0001_0000, // 16
        Center = 0b0000_1000, // 8
        Right1 = 0b0000_0100, // 4
        Right2 = 0b0000_0010, // 2
        Right3 = 0b0000_0001, // 1

        // Convenience combinations
        AllLeft = Left1 | Left2 | Left3,
        AllRight = Right1 | Right2 | Right3,
        AllRoads = AllLeft | Center | AllRight
    }
}