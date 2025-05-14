using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace SplineMesh.SplineMesh.Runtime.Core
{
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
        [FormerlySerializedAs("meshFilter")] public MeshFilter[] meshFilters;

        [FormerlySerializedAs("segmentMesh")] [Space] [Header("Spline Mesh Settings")] [SerializeField]
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



        [Tooltip("Count must match the number of Splines in the Spline Container")]
        [SerializeField]
        private int meshResolutions = 10;

        public float3 upDirection = Vector3.forward;


        private void Reset()
        {
            splineContainer = GetComponent<SplineContainer>();
        }



        [ContextMenu("GenerateMeshAlongSpline")]
        public void GenerateMeshAlongSpline() => GenerateMeshAlongSpline(modelMesh, splineContainer.Splines[0]);

        private void GenerateMeshAlongSpline(Mesh mesh, Spline spline)
        {
            int curveCount = spline.GetCurveCount() - 1;
            int combinedVertexOffset = 0;
            int meshVertexCount = mesh.vertexCount;
            int meshVerticesLength = mesh.vertices.Length;
            float meshBoundsDistance = math.abs(GetRequiredAxis(mesh.bounds.size, forwardAxis));

            var meshNormal = NativeMeshNormal(mesh, Allocator.Temp);
            var meshUV = NativeMeshUV(mesh, Allocator.Temp);
            var combinedSubMeshTriangles = NativeCombinedSubMeshTriangles(mesh, Allocator.Temp);
            var nativeMeshTriangle = NativeMeshTriangle(mesh, Allocator.Temp);

            NativeModelVertexRatioOffset(mesh, meshBoundsDistance, Allocator.Temp, out var vertexRatios,
                out var vertexOffsets);

            NativeSplineCache(spline,
                meshVerticesLength, vertexRatios, meshNormal, Allocator.Temp,
                out var nativeSplinePosTan, out var nativeSplineNormalTan
            );
            
            
            for (var i = 0; i < meshFilters.Length; i++)
            {
                var vertices = new NativeList<float3>(Allocator.Temp);
                var normals = new NativeList<float3>(Allocator.Temp);
                var uvs = new NativeList<float2>(Allocator.Temp);

                Debug.Log($"meshFilters: {i}");
                GetMeshData(nativeSplinePosTan, nativeSplineNormalTan, meshVerticesLength, vertexRatios,
                    vertexOffsets, vertices, meshNormal, normals,
                    meshVertexCount, nativeMeshTriangle, combinedSubMeshTriangles, combinedVertexOffset, meshUV,
                    curveCount,
                    uvs, positionAdjustments[i]);

                SetMesh(meshFilters[i], TVector3S(vertices), TVector3S(normals), TVector2S(uvs),
                    ToInt(combinedSubMeshTriangles));
                vertices.Dispose();
                normals.Dispose();
                uvs.Dispose();
            }

            Dispose(meshNormal, meshUV, combinedSubMeshTriangles, nativeMeshTriangle,
                nativeSplinePosTan, nativeSplineNormalTan);
        }

        private static void Dispose(
            NativeArray<float3> meshNormal,
            NativeArray<float2> meshUV, NativeArray<NativeList<int>> combinedSubMeshTriangles,
            NativeArray<NativeArray<int>> nativeMeshTriangle,
            NativeArray<float3x2> nativeSplinePosTan, NativeArray<float3> nativeSplineNormalTan)
        {
            meshNormal.Dispose();
            meshUV.Dispose();

            foreach (var nativeList in combinedSubMeshTriangles) nativeList.Dispose();
            foreach (var nativeList in nativeMeshTriangle) nativeList.Dispose();
            nativeSplinePosTan.Dispose();
            nativeSplineNormalTan.Dispose();
        }

        private void NativeSplineCache(Spline spline,
            int meshVerticesLength, NativeList<float> vertexRatios, NativeArray<float3> meshNormal, Allocator allocator,
            out NativeArray<float3x2> nativeSplinePosTan, out NativeArray<float3> nativeSplineNormalTan
        )
        {
            nativeSplinePosTan = new NativeArray<float3x2>(meshResolutions * meshVerticesLength, allocator);
            nativeSplineNormalTan = new NativeArray<float3>(meshResolutions * meshNormal.Length, allocator);
            for (int resIncrement = 0, c0 = 0, c1 = 0; resIncrement < meshResolutions; resIncrement++)
            {
                var resolutionFraction = resIncrement / (float)meshResolutions;
                for (var vertexIndex = 0; vertexIndex < meshVerticesLength; vertexIndex++)
                {
                    float t = resolutionFraction + vertexRatios[vertexIndex] * (1 / (float)meshResolutions);
                    Evaluate(spline, t, out float3 splinePosition, out float3 tangent);
                    nativeSplinePosTan[c0++] = new float3x2(splinePosition, tangent);
                }

                // Add transformed normals
                for (int normalIndex = 0; normalIndex < meshNormal.Length; normalIndex++)
                {
                    float point = resolutionFraction + vertexRatios[normalIndex] * (1 / (float)meshResolutions);
                    nativeSplineNormalTan[c1++] = spline.EvaluateTangent(point);
                }
            }
        }

        [BurstCompile]
        private void GetMeshData(NativeArray<float3x2> nativeSplinePosTan, NativeArray<float3> nativeSplineNormalTan,
            int meshVerticesLength, NativeList<float> vertexRatios,
            NativeList<float3> vertexOffsets,
            NativeList<float3> vertices, NativeArray<float3> meshNormal, NativeList<float3> normals,
            int meshVertexCount,
            NativeArray<NativeArray<int>> nativeMeshTriangle, NativeArray<NativeList<int>> combinedSubMeshTriangles,
            int combinedVertexOffset, NativeArray<float2> meshUV,
            int curveCount, NativeList<float2> uvs,
            float3 positionAdjustment)
        {
            for (int resIncrement = 0, c0 = 0, c1 = 0; resIncrement < meshResolutions; resIncrement++)
            {
                var resolutionFraction = resIncrement / (float)meshResolutions;
                for (var vertexIndex = 0; vertexIndex < meshVerticesLength; vertexIndex++)
                {
                    var splineRotation = quaternion.LookRotationSafe(nativeSplinePosTan[c0].c1, upDirection);
                    var verticesPosition =
                        nativeSplinePosTan[c0].c0 + math.mul(splineRotation, vertexOffsets[vertexIndex]);
                    var positionOffset = math.mul(splineRotation, positionAdjustment);
                    vertices.Add(verticesPosition + positionOffset);
                    c0++;
                }

                foreach (var normal in meshNormal)
                {
                    var tangent = nativeSplineNormalTan[c1++];
                    var splineRotation = quaternion.LookRotationSafe(tangent, upDirection);
                    var transformedNormal = math.mul(splineRotation, normal);
                    normals.Add(transformedNormal);
                }

                // Add triangles to each submesh
                AddTriangulation(meshVertexCount, nativeMeshTriangle, combinedSubMeshTriangles,
                    ref combinedVertexOffset);
                // Add UVs with UV resolution
                SetUV(meshUV, resolutionFraction, vertexRatios, resIncrement, curveCount, uvs);
            }
        }


        [BurstCompile]
        private static NativeArray<NativeList<int>> NativeCombinedSubMeshTriangles(Mesh mesh, Allocator allocator)
        {
            var combinedSubMeshTriangles = new NativeArray<NativeList<int>>(mesh.subMeshCount, allocator);
            for (int i = 0; i < mesh.subMeshCount; i++) combinedSubMeshTriangles[i] = new NativeList<int>(allocator);
            return combinedSubMeshTriangles;
        }

        [BurstCompile]
        private static NativeArray<float3> NativeMeshNormal(Mesh mesh, Allocator allocator)
        {
            var meshNormal = new NativeArray<float3>(mesh.normals.Length, allocator);
            for (int i = 0; i < mesh.normals.Length; i++) meshNormal[i] = mesh.normals[i];
            return meshNormal;
        }

        [BurstCompile]
        private static NativeArray<float2> NativeMeshUV(Mesh mesh, Allocator allocator)
        {
            var meshUV = new NativeArray<float2>(mesh.uv.Length, allocator);
            for (int i = 0; i < mesh.uv.Length; i++) meshUV[i] = mesh.uv[i];
            return meshUV;
        }

        [BurstCompile]
        private void SetUV(NativeArray<float2> meshUV, float resolutionFraction, NativeList<float> vertexRatios,
            int resIncrement,
            int curveCount, NativeList<float2> uvs)
        {
            for (int uvIndex = 0; uvIndex < meshUV.Length; uvIndex++)
            {
                var uv = meshUV[uvIndex];
                float point;
                if (uniformUVs) point = resolutionFraction + vertexRatios[uvIndex] * (1 / (float)meshResolutions);
                else point = resIncrement / (float)curveCount + vertexRatios[uvIndex] * (1 / (float)curveCount);
                var splineUV = MakeUVs(uv, point, uvAxis, uvResolutions); // Apply UV resolution
                uvs.Add(splineUV);
            }
        }

        [BurstCompile]
        private static void AddTriangulation(int vertexCount, NativeArray<NativeArray<int>> nativeMeshTriangle,
            NativeArray<NativeList<int>> combinedSubMeshTriangles,
            ref int combinedVertexOffset)
        {
            int meshSubMeshCount = nativeMeshTriangle.Length;
            for (int subMeshIndex = 0; subMeshIndex < meshSubMeshCount; subMeshIndex++)
            {
                // var triangles = mesh.GetTriangles(subMeshIndex);
                var triangles = nativeMeshTriangle[subMeshIndex];

                for (int k = 0; k < triangles.Length; k += 3)
                {
                    combinedSubMeshTriangles[subMeshIndex].Add(triangles[k] + combinedVertexOffset);
                    combinedSubMeshTriangles[subMeshIndex].Add(triangles[k + 2] + combinedVertexOffset);
                    combinedSubMeshTriangles[subMeshIndex].Add(triangles[k + 1] + combinedVertexOffset);
                }
            }

            combinedVertexOffset += vertexCount;
        }

        [BurstCompile]
        private static NativeArray<NativeArray<int>> NativeMeshTriangle(Mesh mesh, Allocator allocator)
        {
            var nativeSubMeshTriangle = new NativeArray<NativeArray<int>>(mesh.subMeshCount, allocator);
            for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
            {
                var triangles = mesh.GetTriangles(subMeshIndex);
                var nativeTriangles = new NativeArray<int>(triangles.Length, allocator);
                for (int i = 0; i < triangles.Length; i++) nativeTriangles[i] = triangles[i];
                nativeSubMeshTriangle[subMeshIndex] = nativeTriangles;
            }

            return nativeSubMeshTriangle;
        }

        private void SetMesh(MeshFilter meshFilter, Vector3[] vertices, Vector3[] normals, Vector2[] uvs,
            int[][] combinedSubMeshTriangles)
        {
            var generatedMesh = new Mesh
            {
                name = $"meshName {meshFilter.gameObject}",
                vertices = vertices,
                normals = normals,
                uv = uvs,
                subMeshCount = modelMesh.subMeshCount
            };

            for (int submeshIndex = 0; submeshIndex < modelMesh.subMeshCount; submeshIndex++)
                generatedMesh.SetTriangles(combinedSubMeshTriangles[submeshIndex], submeshIndex);

            meshFilter.mesh = generatedMesh;

            generatedMesh.RecalculateBounds();
            generatedMesh.RecalculateNormals();
            generatedMesh.RecalculateTangents();
        }

        private static int[][] ToInt(NativeArray<NativeList<int>> nativeList)
        {
            int[][] managedArray = new int[nativeList.Length][];
            for (int i = 0; i < nativeList.Length; i++)
            {
                var list = nativeList[i];
                managedArray[i] = new int[list.Length];
                for (int j = 0; j < list.Length; j++)
                {
                    managedArray[i][j] = list[j];
                }
            }

            return managedArray;
        }

        private static Vector3[] TVector3S(NativeList<float3> nativeList)
        {
            Vector3[] managedArray = new Vector3[nativeList.Length];
            for (int i = 0; i < nativeList.Length; i++) managedArray[i] = nativeList[i];
            return managedArray;
        }

        private static Vector2[] TVector2S(NativeList<float2> nativeList)
        {
            Vector2[] managedArray = new Vector2[nativeList.Length];
            for (int i = 0; i < nativeList.Length; i++) managedArray[i] = nativeList[i];
            return managedArray;
        }

        [BurstCompile]
        private void NativeModelVertexRatioOffset(
            Mesh mesh,
            float meshBoundsDistance,
            Allocator allocator,
            out NativeList<float> vertexRatios,
            out NativeList<float3> vertexOffsets)
        {
            vertexRatios = new NativeList<float>(allocator);
            vertexOffsets = new NativeList<float3>(allocator);

            // Calculate vertex ratios and offsets
            foreach (var vertex in mesh.vertices)
            {
                float ratio = math.abs(GetRequiredAxis(vertex, forwardAxis)) / meshBoundsDistance;
                var offset = GetRequiredOffset(vertex, forwardAxis);
                vertexRatios.Add(ratio);
                vertexOffsets.Add(offset);
            }
        }

        [BurstCompile]
        private static float3 GetRequiredOffset(float3 vector, VectorAxis axis)
        {
            return axis switch
            {
                VectorAxis.X => new float3(vector.y, vector.z, 0f),
                VectorAxis.Y => new float3(vector.x, vector.z, 0f),
                _ => new float3(vector.x, vector.z, 0f)
            };
        }

        [BurstCompile]
        private static float GetRequiredAxis(float3 vector, VectorAxis axis)
        {
            return axis switch
            {
                VectorAxis.X => vector.x,
                VectorAxis.Y => vector.y,
                _ => vector.y
            };
        }

        [BurstCompile]
        private static float2 MakeUVs(float2 uv, float point, VectorAxis uvAxis, float uvResolutions)
        {
            return uvAxis switch
            {
                VectorAxis.X => new float2(point * uvResolutions, uv.y),
                _ => new float2(uv.x, point * uvResolutions)
            };
        }

        [BurstCompile]
        private static void Evaluate(Spline spline,
            float t,
            out float3 position,
            out float3 tangent
        )
        {
            var curveIndex = spline.SplineToCurveT(t, out var curveT);
            BezierCurve curve = spline.GetCurve(curveIndex);

            position = CurveUtility.EvaluatePosition(curve, curveT);
            tangent = CurveUtility.EvaluateTangent(curve, curveT);
        }


        [BurstCompile]
        private static void Evaluate(
            BezierCurve curve,
            float curveT,
            out float3 position,
            out float3 tangent
        )
        {
            position = CurveUtility.EvaluatePosition(curve, curveT);
            tangent = CurveUtility.EvaluateTangent(curve, curveT);
        }
    }
}