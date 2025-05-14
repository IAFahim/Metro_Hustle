using System.Collections.Generic;
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

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(SplineContainer)), DisallowMultipleComponent,
     ExecuteInEditMode]
    public class SplineMeshResolution : MonoBehaviour
    {
        public SplineContainer splineContainer;
        public MeshFilter meshFilter;

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
        protected Vector3 positionAdjustment;


        [FormerlySerializedAs("meshResolution")]
        [Tooltip("Count must match the number of Splines in the Spline Container")]
        [SerializeField]
        private int meshResolutions = 10;

        public float3 upDirection = Vector3.forward;


        private void Reset()
        {
            splineContainer = GetComponent<SplineContainer>();
            meshFilter = GetComponent<MeshFilter>();
        }


        [ContextMenu("GenerateMeshAlongSpline")]
        public void GenerateMeshAlongSpline() => GenerateMeshAlongSpline(modelMesh, splineContainer.Splines[0]);

        private void GenerateMeshAlongSpline(Mesh mesh, Spline spline)
        {
            int curveCount = spline.GetCurveCount() - 1;

            int combinedVertexOffset = 0;
            var vertices = new NativeList<float3>(Allocator.Temp);
            var normals = new NativeList<float3>();
            var uvs = new NativeList<float2>(Allocator.Temp);
            var combinedSubMeshTriangles = new NativeList<int>[mesh.subMeshCount];
            for (int i = 0; i < mesh.subMeshCount; i++)
                combinedSubMeshTriangles[i] = new NativeList<int>(Allocator.Temp);

            float meshBoundsDistance = Mathf.Abs(GetRequiredAxis(mesh.bounds.size, forwardAxis));
            GetModelVertexRatioOffset(mesh, meshBoundsDistance, out var vertexRatios, out var vertexOffsets);

            for (int resIncrement = 0; resIncrement < meshResolutions; resIncrement++)
            {
                var resolutionFraction = resIncrement / (float)meshResolutions;
                for (var vertexIndex = 0; vertexIndex < mesh.vertices.Length; vertexIndex++)
                {
                    float t = resolutionFraction + vertexRatios[vertexIndex] * (1 / (float)meshResolutions);
                    Evaluate(spline, t, out float3 splinePosition, out var tangent);

                    var splineRotation = quaternion.LookRotationSafe(tangent, upDirection);
                    var transformedPosition = splinePosition + math.mul(splineRotation, vertexOffsets[vertexIndex]);
                    vertices.Add(transformedPosition + math.mul(splineRotation, positionAdjustment));
                }

                // Add transformed normals
                for (int normalIndex = 0; normalIndex < mesh.normals.Length; normalIndex++)
                {
                    var normal = mesh.normals[normalIndex];
                    float point = (resolutionFraction) + (vertexRatios[normalIndex] * (1 / (float)meshResolutions));
                    var tangent = spline.EvaluateTangent(point);
                    var splineRotation = quaternion.LookRotationSafe(tangent, upDirection);
                    var transformedNormal = math.mul(splineRotation, normal);
                    normals.Add(transformedNormal);
                }

                // Add triangles to each submesh
                AddTriangulation(mesh, combinedSubMeshTriangles, ref combinedVertexOffset);
                // Add UVs with UV resolution
                SetUV(mesh, resolutionFraction, vertexRatios, resIncrement, curveCount, uvs);
            }


            SetMesh(TVector3S(vertices), TVector3S(normals), TVector2S(uvs), ToInt(combinedSubMeshTriangles));
            vertices.Dispose();
            uvs.Dispose();
            foreach (var nativeList in combinedSubMeshTriangles) nativeList.Dispose();
        }

        private void SetUV(Mesh mesh, float resolutionFraction, List<float> vertexRatios, int resIncrement,
            int curveCount, NativeList<float2> uvs)
        {
            for (int j = 0; j < mesh.uv.Length; j++)
            {
                var uv = mesh.uv[j];
                float point;
                if (uniformUVs) point = resolutionFraction + vertexRatios[j] * (1 / (float)meshResolutions);
                else point = resIncrement / (float)curveCount + vertexRatios[j] * (1 / (float)curveCount);
                var splineUV = MakeUVs(uv, point, uvAxis, uvResolutions); // Apply UV resolution
                uvs.Add(splineUV);
            }
        }

        private static void AddTriangulation(Mesh mesh, NativeList<int>[] combinedSubMeshTriangles,
            ref int combinedVertexOffset)
        {
            for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
            {
                var subMeshIndices = mesh.GetTriangles(subMeshIndex);

                for (int k = 0; k < subMeshIndices.Length; k += 3)
                {
                    combinedSubMeshTriangles[subMeshIndex].Add(subMeshIndices[k] + combinedVertexOffset);
                    combinedSubMeshTriangles[subMeshIndex].Add(subMeshIndices[k + 2] + combinedVertexOffset);
                    combinedSubMeshTriangles[subMeshIndex].Add(subMeshIndices[k + 1] + combinedVertexOffset);
                }
            }

            combinedVertexOffset += mesh.vertexCount;
        }

        private void SetMesh(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[][] combinedSubMeshTriangles)
        {
            var generatedMesh = new Mesh
            {
                name = meshName,
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

        private static int[][] ToInt(NativeList<int>[] nativeList)
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
            for (int i = 0; i < nativeList.Length; i++)
            {
                managedArray[i] = nativeList[i];
            }

            return managedArray;
        }

        private static Vector2[] TVector2S(NativeList<float2> nativeList)
        {
            Vector2[] managedArray = new Vector2[nativeList.Length];
            for (int i = 0; i < nativeList.Length; i++)
            {
                managedArray[i] = nativeList[i];
            }

            return managedArray;
        }

        private void GetModelVertexRatioOffset(
            Mesh mesh,
            float meshBoundsDistance,
            out List<float> vertexRatios,
            out List<Vector3> vertexOffsets)
        {
            vertexRatios = new List<float>();
            vertexOffsets = new List<Vector3>();

            // Calculate vertex ratios and offsets
            foreach (var vertex in mesh.vertices)
            {
                float ratio = Mathf.Abs(GetRequiredAxis(vertex, forwardAxis)) / meshBoundsDistance;
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
                VectorAxis.X => new Vector3(vector.y, vector.z, 0f),
                VectorAxis.Y => new Vector3(vector.x, vector.z, 0f),
                _ => new Vector3(vector.x, vector.z, 0f)
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
            var curve = spline.GetCurve(curveIndex);

            position = CurveUtility.EvaluatePosition(curve, curveT);
            tangent = CurveUtility.EvaluateTangent(curve, curveT);
        }
    }
}