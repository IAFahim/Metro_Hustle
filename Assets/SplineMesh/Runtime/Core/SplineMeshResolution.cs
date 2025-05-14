using System.Collections.Generic;
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

        public Vector3 upDirection = Vector3.forward;


        private void Reset()
        {
            splineContainer = GetComponent<SplineContainer>();
            meshFilter = GetComponent<MeshFilter>();
        }


        [ContextMenu("GenerateMeshAlongSpline")]
        public void GenerateMeshAlongSpline(Mesh mesh)
        {
            var spline = splineContainer.Splines[0];
            int curveCount = spline.GetCurveCount() - 1;

            int combinedVertexOffset = 0;
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            var combinedSubmeshTriangles = new List<int>[mesh.subMeshCount];
            for (int i = 0; i < mesh.subMeshCount; i++) combinedSubmeshTriangles[i] = new List<int>();

            float meshBoundsDistance = Mathf.Abs(GetRequiredAxis(mesh.bounds.size, forwardAxis));
            GetModelVertexRatioOffset(mesh, meshBoundsDistance, out var vertexRatios, out var vertexOffsets);

            for (int resIncrement = 0; resIncrement < meshResolutions; resIncrement++)
            {
                var resolutionFraction = resIncrement / (float)meshResolutions;
                for (var vertexIndex = 0; vertexIndex < mesh.vertices.Length; vertexIndex++)
                {
                    float t = resolutionFraction + vertexRatios[vertexIndex] * (1 / (float)meshResolutions);
                    Evaluate(spline, t, out float3 splinePosition, out var tangent);

                    var splineRotation = Quaternion.LookRotation(tangent, upDirection);
                    var transformedPosition = (Vector3)splinePosition + splineRotation * vertexOffsets[vertexIndex];
                    vertices.Add(transformedPosition + splineRotation * positionAdjustment);
                }

                // Add transformed normals
                for (int normalIndex = 0; normalIndex < mesh.normals.Length; normalIndex++)
                {
                    var normal = mesh.normals[normalIndex];
                    float point = (resolutionFraction) + (vertexRatios[normalIndex] * (1 / (float)meshResolutions));
                    var tangent = spline.EvaluateTangent(point);
                    var splineRotation = Quaternion.LookRotation(tangent, upDirection);
                    var transformedNormal = splineRotation * normal;
                    normals.Add(transformedNormal);
                }

                // Add triangles to each submesh
                for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
                {
                    var subMeshIndices = mesh.GetTriangles(subMeshIndex);

                    for (int k = 0; k < subMeshIndices.Length; k += 3)
                    {
                        combinedSubmeshTriangles[subMeshIndex].Add(subMeshIndices[k] + combinedVertexOffset);
                        combinedSubmeshTriangles[subMeshIndex].Add(subMeshIndices[k + 2] + combinedVertexOffset);
                        combinedSubmeshTriangles[subMeshIndex].Add(subMeshIndices[k + 1] + combinedVertexOffset);
                    }
                }

                combinedVertexOffset += mesh.vertexCount;

                // Add UVs with UV resolution

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


            SetMesh(vertices.ToArray(), normals.ToArray(), uvs.ToArray(), combinedSubmeshTriangles);
        }

        private void SetMesh(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, List<int>[] combinedSubmeshTriangles)
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
                generatedMesh.SetTriangles(combinedSubmeshTriangles[submeshIndex].ToArray(), submeshIndex);

            meshFilter.mesh = generatedMesh;

            generatedMesh.RecalculateBounds();
            generatedMesh.RecalculateNormals();
            generatedMesh.RecalculateTangents();
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

        private static Vector3 GetRequiredOffset(Vector3 vector, VectorAxis axis)
        {
            return axis switch
            {
                VectorAxis.X => new Vector3(vector.y, vector.z, 0f),
                VectorAxis.Y => new Vector3(vector.x, vector.z, 0f),
                _ => new Vector3(vector.x, vector.z, 0f)
            };
        }

        private static float GetRequiredAxis(Vector3 vector, VectorAxis axis)
        {
            return axis switch
            {
                VectorAxis.X => vector.x,
                VectorAxis.Y => vector.y,
                _ => vector.y
            };
        }

        private static Vector2 MakeUVs(Vector2 uv, float point, VectorAxis uvAxis, float uvResolutions)
        {
            return uvAxis switch
            {
                VectorAxis.X => new Vector2(point * uvResolutions, uv.y),
                _ => new Vector2(uv.x, point * uvResolutions)
            };
        }

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