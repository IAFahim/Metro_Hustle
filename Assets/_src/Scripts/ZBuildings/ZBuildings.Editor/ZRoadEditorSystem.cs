using _src.Scripts.ZBuildings.ZBuildings.Data; // Assuming ZRoadComponent is here
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine; // For Color
#if ALINE
using Drawing; // For CommandBuilder
#endif

namespace _src.Scripts.ZBuildings.ZBuildings.Editor // Or your preferred editor namespace
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(PresentationSystemGroup))] // Or another suitable editor update group
    public partial struct ZRoadVisualizerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // No specific creation needed for this simple visualizer
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
#if ALINE
            // Ensure there's at least one ZRoadComponent to avoid unnecessary builder creation
            // This is a micro-optimization, can be skipped if preferred.
            var query = SystemAPI.QueryBuilder().WithAll<ZRoadComponent, LocalToWorld>().Build();
            if (query.IsEmpty)
            {
                return;
            }

            var builder = DrawingManager.GetBuilder();

            // Optional: Pass editor camera rotation if you plan to add labels that need to face the camera
            // quaternion editorCamRot = quaternion.identity;
            // if (UnityEditor.SceneView.lastActiveSceneView != null)
            // {
            //    editorCamRot = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
            // }

            var job = new ZRoadVisualizerJob
            {
                Drawing = builder
                // EditorCameraRotation = editorCamRot // If using labels
            };

            state.Dependency = job.ScheduleParallel(query, state.Dependency); // Schedule with query
            builder.DisposeAfter(state.Dependency);
#endif
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }

    [BurstCompile]  
    public partial struct ZRoadVisualizerJob : IJobEntity
    {
        public CommandBuilder Drawing;

        private static readonly Color LaneColor = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        private static readonly Color CenterLaneColor = new Color(1f, 0.9f, 0.4f, 0.9f);
        private static readonly Color SideGapMarkerColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);

        private static readonly RoadFlag[] AllPossibleSingleLanes = new[] {
            RoadFlag.Left3, RoadFlag.Left2, RoadFlag.Left1, RoadFlag.Center,
            RoadFlag.Right1, RoadFlag.Right2, RoadFlag.Right3
        };

        void Execute(in ZRoadComponent road, in LocalToWorld ltw)
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
                    if (activeLaneCount > 0) {
                        leftGapMarkerActualCenterX = leftmostLaneEdgeX - (sideGap / 2f);
                    } else { // Only gaps exist, center them around roadBedCenterX
                        leftGapMarkerActualCenterX = roadBedCenterX - (sideGap / 2f);
                    }
                    float3 leftGapMarkerCenterPos = new float3(leftGapMarkerActualCenterX, gapVisualOffset, 0);
                    float3 gapMarkerBoxSize = new float3(sideGap, markerThickness, sizeZ);
                    Drawing.SolidBox(leftGapMarkerCenterPos, gapMarkerBoxSize, SideGapMarkerColor);

                    // Right gap
                    float rightGapMarkerActualCenterX;
                     if (activeLaneCount > 0) {
                        rightGapMarkerActualCenterX = rightmostLaneEdgeX + (sideGap / 2f);
                    } else { // Only gaps exist
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