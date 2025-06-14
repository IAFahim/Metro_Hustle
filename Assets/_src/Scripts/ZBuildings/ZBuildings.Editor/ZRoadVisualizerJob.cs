#if ALINE
using _src.Scripts.ZBuildings.ZBuildings.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace _src.Scripts.ZBuildings.ZBuildings.Editor
{
    [BurstCompile]
    public partial struct ZRoadVisualizerJob : IJobEntity
    {
        public Drawing.CommandBuilder Drawing;

        private static readonly Color LaneColor = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        private static readonly Color CenterLaneColor = new Color(1f, 0.9f, 0.4f, 0.9f);
        private static readonly Color SideGapMarkerColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);

        private static readonly RoadFlag[] AllPossibleSingleLanes = new[]
        {
            RoadFlag.Left3, RoadFlag.Left2, RoadFlag.Left1, RoadFlag.Center,
            RoadFlag.Right1, RoadFlag.Right2, RoadFlag.Right3
        };

        void Execute(in RoadComponent road, in LocalToWorld ltw)
        {
            using (Drawing.WithMatrix(ltw.Value))
            {
                float sizeZ = road.SizeZ;
                float sideGap = road.SideGap;
                float perLineWidth = road.PerLineWidth;

                int activeLaneCount = 0;
                float leftmostLaneEdgeX = float.MaxValue;
                float rightmostLaneEdgeX = float.MinValue;

                // Determine the extent of the active lanes
                foreach (RoadFlag laneFlag in AllPossibleSingleLanes)
                {
                    if (road.HasFlagFast(laneFlag))
                    {
                        activeLaneCount++;
                        int laneIndex = road.GetFlagPosition(laneFlag);
                        if (laneIndex != int.MaxValue)
                        {
                            float laneCenterX = laneIndex * perLineWidth;
                            float currentLaneLeftEdge = laneCenterX - (perLineWidth / 2f);
                            float currentLaneRightEdge = laneCenterX + (perLineWidth / 2f);

                            if (currentLaneLeftEdge < leftmostLaneEdgeX) leftmostLaneEdgeX = currentLaneLeftEdge;
                            if (currentLaneRightEdge > rightmostLaneEdgeX) rightmostLaneEdgeX = currentLaneRightEdge;
                        }
                    }
                }

                float totalRoadBedWidth;
                float roadBedCenterX;

                if (activeLaneCount > 0)
                {
                    float lanesActualSpan = rightmostLaneEdgeX - leftmostLaneEdgeX;
                    totalRoadBedWidth = lanesActualSpan + (sideGap * 2f);
                    roadBedCenterX = (leftmostLaneEdgeX + rightmostLaneEdgeX) / 2f; // Center of the lane block
                }
                else if (sideGap > 0.001f) // No lanes, but there are side gaps
                {
                    totalRoadBedWidth = sideGap * 2f;
                    roadBedCenterX = 0f; // Center the gaps if no lanes
                }
                else // No lanes and no gaps
                {
                    // Optional: Draw a placeholder or just return
                    // Drawing.SphereOutline(float3.zero, 0.1f, Color.magenta);
                    return;
                }

                if (totalRoadBedWidth <= 0.001f) return;


                // --- Draw Road Bed ---
                float roadBedYLevel = -0.01f;
                float roadBedThickness = 0.01f;

                // --- Draw Side Gaps ---
                float markerYLevel = roadBedYLevel + roadBedThickness;
                float markerThickness = 0.005f;

                if (sideGap > 0.001f)
                {
                    float gapVisualOffset = markerYLevel + markerThickness / 2f;

                    // Left gap: Position it to the left of the leftmost lane, or to the left of the roadBedCenter if no lanes
                    float leftGapMarkerActualCenterX;
                    if (activeLaneCount > 0)
                    {
                        leftGapMarkerActualCenterX = leftmostLaneEdgeX - (sideGap / 2f);
                    }
                    else
                    {
                        // Only gaps exist, center them around roadBedCenterX
                        leftGapMarkerActualCenterX = roadBedCenterX - (sideGap / 2f);
                    }

                    float3 leftGapMarkerCenterPos = new float3(leftGapMarkerActualCenterX, gapVisualOffset, 0);
                    float3 gapMarkerBoxSize = new float3(sideGap, markerThickness, sizeZ);
                    Drawing.SolidBox(leftGapMarkerCenterPos, gapMarkerBoxSize, SideGapMarkerColor);

                    // Right gap
                    float rightGapMarkerActualCenterX;
                    if (activeLaneCount > 0)
                    {
                        rightGapMarkerActualCenterX = rightmostLaneEdgeX + (sideGap / 2f);
                    }
                    else
                    {
                        // Only gaps exist
                        rightGapMarkerActualCenterX = roadBedCenterX + (sideGap / 2f);
                    }

                    float3 rightGapMarkerCenterPos = new float3(rightGapMarkerActualCenterX, gapVisualOffset, 0);
                    Drawing.SolidBox(rightGapMarkerCenterPos, gapMarkerBoxSize, SideGapMarkerColor);
                }

                // --- Draw individual lanes ---
                float laneYLevel = markerYLevel + markerThickness;
                float laneThickness = 0.01f;

                if (activeLaneCount > 0)
                {
                    foreach (RoadFlag laneFlag in AllPossibleSingleLanes)
                    {
                        if (road.HasFlagFast(laneFlag))
                        {
                            int laneIndex = road.GetFlagPosition(laneFlag);
                            if (laneIndex == int.MaxValue) continue;

                            float offsetX = laneIndex * perLineWidth;
                            Color currentLaneDrawColor = (laneFlag == RoadFlag.Center) ? CenterLaneColor : LaneColor;

                            float3 laneCenter = new float3(offsetX, laneYLevel + laneThickness / 2f, 0);
                            float3 laneSize = new float3(perLineWidth, laneThickness, sizeZ);
                            Drawing.SolidBox(laneCenter, laneSize, currentLaneDrawColor);
                        }
                    }
                }
            }
        }
    }
}
#endif