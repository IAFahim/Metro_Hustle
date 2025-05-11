using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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
        protected float uvResolutions;

        [Space] [Header("Offsets")] [SerializeField]
        protected Vector3 positionAdjustment;
        [SerializeField] protected Quaternion rotationAdjustment;
        [SerializeField] protected Vector3 scaleAdjustment = Vector3.one;

        
        [Tooltip("Count must match the number of Splines in the Spline Container")] [SerializeField]
        private int meshResolution;


        private void Reset()
        {
            splineContainer = GetComponent<SplineContainer>();
            meshFilter = GetComponent<MeshFilter>();
        }
        
        

        
        [MenuItem("GenerateMeshAlongSpline")]

        public void GenerateMeshAlongSpline()
        {
            var combinedVertices = new List<Vector3>();
            var combinedNormals = new List<Vector3>();
            var combinedUVs = new List<Vector2>();
            var combinedSubmeshTriangles = new List<int>[segmentMesh.subMeshCount];

            for (int i = 0; i < segmentMesh.subMeshCount; i++)
                combinedSubmeshTriangles[i] = new List<int>();

            int combinedVertexOffset = 0;

            var normalizedSegmentMesh = segmentMesh.NormalizeMesh(rotationAdjustment, scaleAdjustment);


            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            var spline = splineContainer.Splines[0];

            for (int i = 0; i < meshResolution; i++)
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

                int counter = 0;

                foreach (var vertex in normalizedSegmentMesh.vertices)
                {
                    float point = (i / (float)meshResolution) +
                                  (vertexRatios[counter] * (1 / (float)meshResolution));
                    var tangent = spline.EvaluateTangent(point);
                    Vector3 splinePosition = spline.EvaluatePosition(point);

                    var splineRotation = Quaternion.LookRotation(tangent, Vector3.up);
                    var transformedPosition = splinePosition + splineRotation * vertexOffsets[counter];

                    vertices.Add(transformedPosition + positionAdjustment);
                    counter++;
                }

                // Add transformed normals
                for (int j = 0; j < normalizedSegmentMesh.normals.Length; j++)
                {
                    var normal = normalizedSegmentMesh.normals[j];
                    float point = (i / (float)meshResolution) +
                                  (vertexRatios[j] * (1 / (float)meshResolution));

                    var tangent = spline.EvaluateTangent(point);
                    var splineRotation = Quaternion.LookRotation(tangent, Vector3.up);
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

                    if (uniformUVs)
                    {
                        point = i / (float)meshResolution + vertexRatios[j] * (1 / (float)meshResolution);
                    }
                    else
                    {
                        point = i / (float)segmentCount + vertexRatios[j] * (1 / (float)segmentCount);
                    }

                    var splineUV = SplineMeshUtils.MakeUVs(uv, point, uvAxis, uvResolutions); // Apply UV resolution
                    uvs.Add(splineUV);
                }

                combinedVertexOffset += normalizedSegmentMesh.vertexCount;
            }

            combinedVertices.AddRange(vertices);
            combinedNormals.AddRange(normals);
            combinedUVs.AddRange(uvs);


            var generatedMesh = new Mesh();
            generatedMesh.name = meshName;
            generatedMesh.vertices = combinedVertices.ToArray();
            generatedMesh.normals = combinedNormals.ToArray();
            generatedMesh.uv = combinedUVs.ToArray();
            generatedMesh.subMeshCount = segmentMesh.subMeshCount;

            for (int submeshIndex = 0; submeshIndex < segmentMesh.subMeshCount; submeshIndex++)
                generatedMesh.SetTriangles(combinedSubmeshTriangles[submeshIndex].ToArray(), submeshIndex);

            meshFilter.mesh = generatedMesh;

            generatedMesh.RecalculateBounds();
            generatedMesh.RecalculateNormals();
            generatedMesh.RecalculateTangents();
        }
    }
}