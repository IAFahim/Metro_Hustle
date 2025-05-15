using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace SplineMesh.SplineMesh.Runtime.Core
{
    public struct SplineCache : IDisposable
    {
        public NativeArray<float3x2> PositionTangent;
        public NativeArray<float3> NormalTangent;

        public SplineCache(Spline spline, int meshResolutions, MeshAssetInfo meshAssetInfo, Allocator allocator)
        {
            PositionTangent =
                new NativeArray<float3x2>(meshResolutions * meshAssetInfo.VertexOffsets.Length, allocator);
            NormalTangent = new NativeArray<float3>(meshResolutions * meshAssetInfo.Normals.Length, allocator);

            for (int resIncrement = 0, pIndex = 0, nIndex = 0; resIncrement < meshResolutions; resIncrement++)
            {
                var resolutionFraction = resIncrement / (float)meshResolutions;

                for (var vertexIndex = 0; vertexIndex < meshAssetInfo.VertexOffsets.Length; vertexIndex++)
                {
                    float t = resolutionFraction + meshAssetInfo.VertexRatios[vertexIndex] * (1f / meshResolutions);
                    MeshUtil.Evaluate(spline, t, out float3 splinePosition, out float3 tangent);
                    PositionTangent[pIndex++] = new float3x2(splinePosition, tangent);
                }

                for (int normalIndex = 0; normalIndex < meshAssetInfo.Normals.Length; normalIndex++)
                {
                    float point = resolutionFraction + meshAssetInfo.VertexRatios[normalIndex] * (1f / meshResolutions);
                    NormalTangent[nIndex++] = math.normalize(spline.EvaluateTangent(point));
                }
            }
        }

        public void Dispose()
        {
            if (PositionTangent.IsCreated) PositionTangent.Dispose();
            if (NormalTangent.IsCreated) NormalTangent.Dispose();
        }
    }

    public struct MeshAssetInfo : IDisposable
    {
        public NativeArray<float3> Normals;
        public NativeArray<float2> UV;
        public NativeArray<NativeArray<int>> Triangles;
        public NativeList<float> VertexRatios;
        public NativeList<float3> VertexOffsets;

        public MeshAssetInfo(Mesh mesh, float meshForwardDistance, VectorAxis forward, Allocator allocator)
        {
            Normals = MeshUtil.CreateNativeArray(mesh.normals, Allocator.Temp);
            UV = MeshUtil.CreateNativeArray(mesh.uv, Allocator.Temp);
            VertexRatioOffset(mesh, meshForwardDistance, forward, Allocator.Temp, out VertexRatios, out VertexOffsets);
            Triangles = NativeMeshTriangle(mesh, allocator);
        }

        private static void VertexRatioOffset(
            Mesh mesh,
            float meshForwardDistance,
            VectorAxis forward,
            Allocator allocator,
            out NativeList<float> vertexRatios,
            out NativeList<float3> vertexOffsets)
        {
            int vertexCount = mesh.vertices.Length;
            vertexRatios = new NativeList<float>(vertexCount, allocator);
            vertexOffsets = new NativeList<float3>(vertexCount, allocator);

            for (int i = 0; i < vertexCount; ++i)
            {
                var vertex = mesh.vertices[i];
                float ratio = math.abs(MeshUtil.GetRequiredAxis(vertex, forward)) / meshForwardDistance;
                var offset = MeshUtil.GetRequiredOffset(vertex, forward);
                vertexRatios.Add(ratio);
                vertexOffsets.Add(offset);
            }
        }

        private static NativeArray<NativeArray<int>> NativeMeshTriangle(Mesh mesh, Allocator allocator)
        {
            var nativeSubMeshTriangles = new NativeArray<NativeArray<int>>(mesh.subMeshCount, allocator);
            for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
            {
                var triangles = mesh.GetTriangles(subMeshIndex);
                var nativeTriangles = new NativeArray<int>(triangles.Length, allocator);
                nativeTriangles.CopyFrom(triangles);
                nativeSubMeshTriangles[subMeshIndex] = nativeTriangles;
            }

            return nativeSubMeshTriangles;
        }


        public void Dispose()
        {
            if (Normals.IsCreated) Normals.Dispose();
            if (UV.IsCreated) UV.Dispose();
            if (VertexRatios.IsCreated) VertexRatios.Dispose();
            if (VertexOffsets.IsCreated) VertexOffsets.Dispose();
            foreach (var nativeArr in Triangles) nativeArr.Dispose();
            if (Triangles.IsCreated) Triangles.Dispose();
        }
    }


    [BurstCompile]
    public struct MeshBakeInfo : IDisposable
    {
        public NativeList<float3> Vertices;
        public NativeList<float3> Normals;
        public NativeList<float2> UV;
        public NativeArray<NativeList<int>> SubMeshTriangles;


        public MeshBakeInfo(int subMeshCount, Allocator allocator)
        {
            Vertices = new NativeList<float3>(allocator);
            Normals = new NativeList<float3>(allocator);
            UV = new NativeList<float2>(allocator);
            SubMeshTriangles = InitSubMeshTriangles(subMeshCount, allocator);
        }


        [BurstCompile]
        private static NativeArray<NativeList<int>> InitSubMeshTriangles(int subMeshCount, Allocator allocator)
        {
            var combinedSubMeshTriangles = new NativeArray<NativeList<int>>(subMeshCount, allocator);
            for (int i = 0; i < subMeshCount; i++) combinedSubMeshTriangles[i] = new NativeList<int>(allocator);
            return combinedSubMeshTriangles;
        }

        public void Dispose()
        {
            if (Vertices.IsCreated) Vertices.Dispose();
            if (Normals.IsCreated) Normals.Dispose();
            if (UV.IsCreated) UV.Dispose();
            foreach (var nativeList in SubMeshTriangles) nativeList.Dispose();
            if (SubMeshTriangles.IsCreated) SubMeshTriangles.Dispose();
        }
    }

    [BurstCompile]
    public struct MakeMeshJob : IJob
    {
        [ReadOnly] public SplineCache SplineCache;
        [ReadOnly] public MeshAssetInfo meshAssetInfo;

        public bool uniformUVs;
        public VectorAxis uvAxis;
        public float uvResolutions;
        public int curveCount;
        public MeshBakeInfo bakeInfo;
        public int meshResolutions;
        public int sourceMeshVerticesLength;
        public int sourceMeshVertexCount;
        public int initialCombinedVertexOffset;
        public float upDirection;
        public float3 positionAdjustment;

        public void Execute()
        {
            int currentVertexOffset = initialCombinedVertexOffset;

            for (int resIncrement = 0, c0 = 0, c1 = 0; resIncrement < meshResolutions; resIncrement++)
            {
                var resolutionFraction = resIncrement / (float)meshResolutions;
                for (var vertexIndex = 0; vertexIndex < sourceMeshVerticesLength; vertexIndex++)
                {
                    var splineRotation =
                        quaternion.LookRotationSafe(SplineCache.PositionTangent[c0].c1, upDirection);
                    var vertexOffset = math.mul(splineRotation, meshAssetInfo.VertexOffsets[vertexIndex]);
                    var verticesPosition = SplineCache.PositionTangent[c0].c0 + vertexOffset;
                    var adjustment = math.mul(splineRotation, positionAdjustment);
                    bakeInfo.Vertices.Add(verticesPosition + adjustment);
                    c0++;
                }

                for (int normalIndex = 0; normalIndex < meshAssetInfo.Normals.Length; normalIndex++)
                {
                    var tangent = SplineCache.NormalTangent[c1++];
                    var splineRotation = quaternion.LookRotationSafe(tangent, upDirection);
                    var transformedNormal = math.mul(splineRotation, meshAssetInfo.Normals[normalIndex]);
                    bakeInfo.Normals.Add(transformedNormal);
                }

                AddTriangulation(sourceMeshVertexCount, meshAssetInfo.Triangles, bakeInfo.SubMeshTriangles,
                    ref currentVertexOffset);

                SetUV(resolutionFraction, meshAssetInfo.VertexRatios, resIncrement, bakeInfo.UV);
            }
        }

        [BurstCompile]
        private void AddTriangulation(int meshVertexCount,
            NativeArray<NativeArray<int>> sourcePerSubMeshTriangles,
            NativeArray<NativeList<int>> outputBakedSubMeshTriangles,
            ref int currentCombinedVertexOffset)
        {
            for (int subMeshIndex = 0; subMeshIndex < sourcePerSubMeshTriangles.Length; subMeshIndex++)
            {
                var sourceTriangles = sourcePerSubMeshTriangles[subMeshIndex];
                var outputTriangleList = outputBakedSubMeshTriangles[subMeshIndex];

                for (int k = 0; k < sourceTriangles.Length; k += 3)
                {
                    outputTriangleList.Add(sourceTriangles[k] + currentCombinedVertexOffset);
                    outputTriangleList.Add(sourceTriangles[k + 2] + currentCombinedVertexOffset);
                    outputTriangleList.Add(sourceTriangles[k + 1] + currentCombinedVertexOffset);
                }
            }

            currentCombinedVertexOffset += meshVertexCount;
        }

        [BurstCompile]
        private void SetUV(float resolutionFraction, NativeList<float> vertexRatios, int resIncrement,
            NativeList<float2> bakedUVsList)
        {
            for (int uvIndex = 0; uvIndex < meshAssetInfo.UV.Length; uvIndex++)
            {
                var uv = meshAssetInfo.UV[uvIndex];
                float point;
                float vertexRatio = vertexRatios[uvIndex];

                if (uniformUVs)
                    point = resolutionFraction + vertexRatio * (1f / meshResolutions);
                else
                    point = (float)resIncrement / curveCount + vertexRatio * (1f / curveCount);

                var splineUV = MakeUVs(uv, point);
                bakedUVsList.Add(splineUV);
            }
        }

        [BurstCompile]
        private float2 MakeUVs(float2 uv, float point)
        {
            return uvAxis switch
            {
                VectorAxis.X => new float2(point * uvResolutions, uv.y),
                _ => new float2(uv.x, point * uvResolutions)
            };
        }
    }

    public enum VectorAxis
    {
        X,
        Y
    }

    [RequireComponent(typeof(SplineContainer))]
    [DisallowMultipleComponent]
    public class SplineMeshResolution : MonoBehaviour
    {
        public SplineContainer splineContainer;
        public MeshFilter[] meshFilters;

        [FormerlySerializedAs("meshFilter")] [Space] [Header("Spline Mesh Settings")] [SerializeField]
        protected Mesh modelMesh;

        [Tooltip("The name of the mesh to be generated")] [SerializeField]
        protected string meshName;

        [Tooltip("The local axis along which the mesh extends")] [SerializeField]
        protected VectorAxis forwardAxis;

        [Tooltip("Axis for the UV to be stretched")] [SerializeField]
        protected VectorAxis uvAxis;

        [Tooltip("Whether UVs are uniformly spread out, or based on the spline points")] [SerializeField]
        protected bool uniformUVs;

        [Tooltip("The UV Resolutions along spline(s). Count must match the same number of splines in the container.")]
        [SerializeField]
        protected float uvResolutions = 1;

        [Space] [Header("Offsets")] [SerializeField]
        protected float3[] positionAdjustments =
        {
            new(0, 5, 0),
            new(0, -5, 0)
        };

        [Tooltip("Number of segments to divide the spline into for mesh generation.")] [SerializeField]
        private int meshResolutions = 10;

        public float3 upDirection = Vector3.forward;


        private void Reset()
        {
            splineContainer = GetComponent<SplineContainer>();
        }

        [ContextMenu("GenerateMeshAlongSpline")]
        public void GenerateMeshAlongSpline()
        {
            if (modelMesh == null)
            {
                Debug.LogError("Model Mesh is not assigned.", this);
                return;
            }

            if (splineContainer == null || splineContainer.Splines.Count == 0)
            {
                Debug.LogError("Spline Container is not assigned or has no splines.", this);
                return;
            }

            if (meshFilters == null || meshFilters.Length == 0)
            {
                Debug.LogError("Mesh Filters are not assigned or are empty.", this);
                return;
            }

            if (positionAdjustments == null || positionAdjustments.Length == 0)
            {
                Debug.LogWarning("Position Adjustments are not set. Using default (0,0,0) if needed.", this);
            }


            GenerateMeshAlongSpline(modelMesh, splineContainer.Splines[0], forwardAxis);
        }

        private void GenerateMeshAlongSpline(Mesh mesh, Spline spline, VectorAxis forward)
        {
            int curveCount = spline.GetCurveCount() - 1;
            float meshForwardDistance = math.abs(MeshUtil.GetRequiredAxis(mesh.bounds.size, forward));
            var meshAssetInfo = new MeshAssetInfo(mesh, meshForwardDistance, forward, Allocator.Temp);
            var splineCache = new SplineCache(spline, meshResolutions, meshAssetInfo, Allocator.Temp);


            for (var i = 0; i < meshFilters.Length; i++)
            {
                MeshBakeInfo bakeInfo = new MeshBakeInfo(mesh.subMeshCount, Allocator.Temp);

                int combinedVertexOffsetForCurrentMesh = 0;
                float3 currentPositionAdjustment = positionAdjustments[i];

                GetMeshData(splineCache, meshAssetInfo, mesh.vertices.Length, mesh.vertexCount, ref bakeInfo,
                    combinedVertexOffsetForCurrentMesh, curveCount, currentPositionAdjustment);

                SetMesh(meshFilters[i], TVector3S(bakeInfo.Vertices), TVector3S(bakeInfo.Normals),
                    TVector2S(bakeInfo.UV), ToInt(bakeInfo.SubMeshTriangles));

                bakeInfo.Dispose();
            }

            meshAssetInfo.Dispose();
            splineCache.Dispose();
        }

        [BurstCompile]
        private void GetMeshData(SplineCache splineCache,
            MeshAssetInfo meshAssetInfo,
            int sourceMeshVerticesLength,
            int sourceMeshVertexCount,
            ref MeshBakeInfo bakeInfo,
            int initialCombinedVertexOffset,
            int curveCount, float3 positionAdjustment)
        {
            int currentVertexOffset =
                initialCombinedVertexOffset;

            for (int resIncrement = 0, c0 = 0, c1 = 0; resIncrement < meshResolutions; resIncrement++)
            {
                var resolutionFraction = resIncrement / (float)meshResolutions;
                for (var vertexIndex = 0; vertexIndex < sourceMeshVerticesLength; vertexIndex++)
                {
                    var splineRotation =
                        quaternion.LookRotationSafe(splineCache.PositionTangent[c0].c1, upDirection);
                    var vertexOffset = math.mul(splineRotation, meshAssetInfo.VertexOffsets[vertexIndex]);
                    var verticesPosition = splineCache.PositionTangent[c0].c0 + vertexOffset;
                    var adjustment = math.mul(splineRotation, positionAdjustment);
                    bakeInfo.Vertices.Add(verticesPosition + adjustment);
                    c0++;
                }

                for (int normalIndex = 0; normalIndex < meshAssetInfo.Normals.Length; normalIndex++)
                {
                    var tangent = splineCache.NormalTangent[c1++];
                    var splineRotation = quaternion.LookRotationSafe(tangent, upDirection);
                    var transformedNormal = math.mul(splineRotation, meshAssetInfo.Normals[normalIndex]);
                    bakeInfo.Normals.Add(transformedNormal);
                }

                AddTriangulation(sourceMeshVertexCount, meshAssetInfo.Triangles, bakeInfo.SubMeshTriangles,
                    ref currentVertexOffset);

                SetUV(meshAssetInfo.UV, resolutionFraction, meshAssetInfo.VertexRatios, resIncrement, curveCount,
                    bakeInfo.UV);
            }
        }


        [BurstCompile]
        private void SetUV(
            NativeArray<float2> sourceMeshUV,
            float resolutionFraction, NativeList<float> vertexRatios, int resIncrement,
            int curveCount, NativeList<float2> bakedUVsList)
        {
            for (int uvIndex = 0; uvIndex < sourceMeshUV.Length; uvIndex++)
            {
                var uv = sourceMeshUV[uvIndex];
                float point;
                float vertexRatio = vertexRatios[uvIndex];

                if (uniformUVs) point = resolutionFraction + vertexRatio * (1f / meshResolutions);
                else point = (float)resIncrement / curveCount + vertexRatio * (1f / curveCount);

                var splineUV = MakeUVs(uv, point, uvAxis, uvResolutions);
                bakedUVsList.Add(splineUV);
            }
        }

        [BurstCompile]
        private static void AddTriangulation(
            int sourceMeshVertexCount,
            NativeArray<NativeArray<int>> sourcePerSubMeshTriangles,
            NativeArray<NativeList<int>> outputBakedSubMeshTriangles,
            ref int currentCombinedVertexOffset)
        {
            for (int subMeshIndex = 0; subMeshIndex < sourcePerSubMeshTriangles.Length; subMeshIndex++)
            {
                var sourceTriangles = sourcePerSubMeshTriangles[subMeshIndex];
                var outputTriangleList = outputBakedSubMeshTriangles[subMeshIndex];

                for (int k = 0; k < sourceTriangles.Length; k += 3)
                {
                    outputTriangleList.Add(sourceTriangles[k] + currentCombinedVertexOffset);
                    outputTriangleList.Add(sourceTriangles[k + 2] +
                                           currentCombinedVertexOffset);
                    outputTriangleList.Add(sourceTriangles[k + 1] + currentCombinedVertexOffset);
                }
            }

            currentCombinedVertexOffset += sourceMeshVertexCount;
        }

        private void SetMesh(MeshFilter meshFilter, Vector3[] vertices, Vector3[] normals, Vector2[] uvs,
            int[][] subMeshTriangles)
        {
            var generatedMesh = meshFilter.sharedMesh;
            bool newMesh = false;
            if (generatedMesh == null || generatedMesh.name != $"meshName {meshFilter.gameObject}")
            {
                generatedMesh = new Mesh { name = $"meshName {meshFilter.gameObject}" };
                newMesh = true;
            }

            generatedMesh.Clear();
            generatedMesh.vertices = vertices;
            generatedMesh.normals = normals;
            generatedMesh.uv = uvs;
            generatedMesh.subMeshCount = modelMesh.subMeshCount;

            for (int subMeshIndex = 0; subMeshIndex < modelMesh.subMeshCount; subMeshIndex++)
            {
                generatedMesh.SetTriangles(subMeshTriangles[subMeshIndex], subMeshIndex);
            }

            if (newMesh) meshFilter.mesh = generatedMesh;
            else meshFilter.sharedMesh = generatedMesh;

            generatedMesh.RecalculateBounds();
            generatedMesh.RecalculateNormals();
            generatedMesh.RecalculateTangents();
        }


        [BurstCompile]
        public static float2 MakeUVs(float2 uv, float point, VectorAxis uvAxis, float uvResolutions)
        {
            return uvAxis switch
            {
                VectorAxis.X => new float2(point * uvResolutions, uv.y),
                _ => new float2(uv.x, point * uvResolutions)
            };
        }

        private static Vector3[] TVector3S(NativeList<float3> nativeList)
        {
            return nativeList.AsArray().Reinterpret<Vector3>().ToArray();
        }

        private static Vector2[] TVector2S(NativeList<float2> nativeList)
        {
            return nativeList.AsArray().Reinterpret<Vector2>().ToArray();
        }

        private static int[][] ToInt(NativeArray<NativeList<int>> nativeListsPerSubmesh)
        {
            int[][] managedArray = new int[nativeListsPerSubmesh.Length][];
            for (int i = 0; i < nativeListsPerSubmesh.Length; i++)
            {
                managedArray[i] = nativeListsPerSubmesh[i].AsArray().ToArray();
            }

            return managedArray;
        }
    }

    [BurstCompile]
    public static class MeshUtil
    {
        public static void Evaluate(Spline spline, float t, out float3 position, out float3 tangent)
        {
            t = math.saturate(t);
            var curveIndex = spline.SplineToCurveT(t, out var curveT);
            BezierCurve curve = spline.GetCurve(curveIndex);

            position = CurveUtility.EvaluatePosition(curve, curveT);
            tangent = math.normalize(CurveUtility.EvaluateTangent(curve, curveT));
        }

        [BurstCompile]
        public static float3 GetRequiredOffset(float3 vector, VectorAxis axis)
        {
            return axis switch
            {
                VectorAxis.X => new float3(vector.y, vector.z, 0f),
                VectorAxis.Y => new float3(vector.x, vector.z, 0f),
                _ => new float3(vector.x, vector.z, 0f)
            };
        }

        [BurstCompile]
        public static float GetRequiredAxis(float3 vector, VectorAxis axis)
        {
            return axis switch
            {
                VectorAxis.X => vector.x,
                VectorAxis.Y => vector.y,
                _ => vector.y
            };
        }

        [BurstCompile]
        public static NativeArray<float2> CreateNativeArray(Vector2[] array, Allocator allocator)
        {
            var nativeArray = new NativeArray<float2>(array.Length, allocator);
            for (int i = 0; i < array.Length; i++) nativeArray[i] = array[i];
            return nativeArray;
        }


        [BurstCompile]
        public static NativeArray<float3> CreateNativeArray(Vector3[] array, Allocator allocator)
        {
            var nativeArray = new NativeArray<float3>(array.Length, allocator);
            for (int i = 0; i < array.Length; i++) nativeArray[i] = array[i];
            return nativeArray;
        }
    }
}