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

        [Space] [Header("Spline Mesh Settings")] [SerializeField]
        protected Mesh segmentMesh;

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
        public void GenerateMeshAlongSpline()
        {
            var combinedVertices = new List<Vector3>();
            var combinedNormals = new List<Vector3>();
            var combinedUVs = new List<Vector2>();
            var combinedSubmeshTriangles = new List<int>[segmentMesh.subMeshCount];

            for (int i = 0; i < segmentMesh.subMeshCount; i++) combinedSubmeshTriangles[i] = new List<int>();

            int combinedVertexOffset = 0;
            var normalizedSegmentMesh = segmentMesh;
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            var spline = splineContainer.Splines[0];

            for (int i = 0; i < meshResolutions; i++)
            {
                float meshBoundsDistance =
                    Mathf.Abs(SplineMeshUtils.GetRequiredAxis(normalizedSegmentMesh.bounds.size, forwardAxis));

                var vertexRatios = new List<float>();
                var vertexOffsets = new List<Vector3>();

                // Calculate vertex ratios and offsets
                foreach (var vertex in normalizedSegmentMesh.vertices)
                {
                    float ratio = Mathf.Abs(SplineMeshUtils.GetRequiredAxis(vertex, forwardAxis)) /
                                  meshBoundsDistance;
                    var offset = SplineMeshUtils.GetRequiredOffset(vertex, forwardAxis);
                    vertexRatios.Add(ratio);
                    vertexOffsets.Add(offset);
                }


                for (var counter = 0; counter < normalizedSegmentMesh.vertices.Length; counter++)
                {
                    float t = (i / (float)meshResolutions) +
                              (vertexRatios[counter] * (1 / (float)meshResolutions));
                    Evaluate(spline, t, out float3 splinePosition, out var tangent);

                    var splineRotation = Quaternion.LookRotation(tangent, upDirection);
                    var transformedPosition = (Vector3)splinePosition + splineRotation * vertexOffsets[counter];

                    vertices.Add(transformedPosition + splineRotation * positionAdjustment);
                }

                // Add transformed normals
                for (int j = 0; j < normalizedSegmentMesh.normals.Length; j++)
                {
                    var normal = normalizedSegmentMesh.normals[j];
                    float point = (i / (float)meshResolutions) +
                                  (vertexRatios[j] * (1 / (float)meshResolutions));

                    var tangent = spline.EvaluateTangent(point);
                    var splineRotation = Quaternion.LookRotation(tangent, upDirection);
                    var transformedNormal = splineRotation * normal;
                    normals.Add(transformedNormal);
                }

                // Add triangles to each submesh
                for (int submeshIndex = 0; submeshIndex < normalizedSegmentMesh.subMeshCount; submeshIndex++)
                {
                    var submeshIndices = normalizedSegmentMesh.GetTriangles(submeshIndex);

                    for (int k = 0; k < submeshIndices.Length; k += 3)
                    {
                        combinedSubmeshTriangles[submeshIndex].Add(submeshIndices[k] + combinedVertexOffset);
                        combinedSubmeshTriangles[submeshIndex].Add(submeshIndices[k + 2] + combinedVertexOffset);
                        combinedSubmeshTriangles[submeshIndex].Add(submeshIndices[k + 1] + combinedVertexOffset);
                    }
                }

                // Add UVs with UV resolution

                int segmentCount = spline.GetCurveCount() - 1;
                for (int j = 0; j < normalizedSegmentMesh.uv.Length; j++)
                {
                    var uv = normalizedSegmentMesh.uv[j];
                    float point;
                    if (uniformUVs) point = i / (float)meshResolutions + vertexRatios[j] * (1 / (float)meshResolutions);
                    else point = i / (float)segmentCount + vertexRatios[j] * (1 / (float)segmentCount);
                    var splineUV = SplineMeshUtils.MakeUVs(uv, point, uvAxis, uvResolutions); // Apply UV resolution
                    uvs.Add(splineUV);
                }

                combinedVertexOffset += normalizedSegmentMesh.vertexCount;
            }

            combinedVertices.AddRange(vertices);
            combinedNormals.AddRange(normals);
            combinedUVs.AddRange(uvs);


            var generatedMesh = new Mesh
            {
                name = meshName,
                vertices = combinedVertices.ToArray(),
                normals = combinedNormals.ToArray(),
                uv = combinedUVs.ToArray(),
                subMeshCount = segmentMesh.subMeshCount
            };

            for (int submeshIndex = 0; submeshIndex < segmentMesh.subMeshCount; submeshIndex++)
                generatedMesh.SetTriangles(combinedSubmeshTriangles[submeshIndex].ToArray(), submeshIndex);

            meshFilter.mesh = generatedMesh;

            generatedMesh.RecalculateBounds();
            generatedMesh.RecalculateNormals();
            generatedMesh.RecalculateTangents();
        }

        public static void Evaluate(Spline spline,
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