using UnityEngine;

namespace SplineMesh.SplineMesh.Runtime.Core
{
	public static class SplineMeshUtils
    {
        public static Mesh NormalizeMesh(this Mesh mesh, Quaternion rotationAdjustment, Vector3 scaleAdjustment)
        {
            var normalizedMesh = Object.Instantiate(mesh);
            var vertices = normalizedMesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector3.Scale(vertices[i], scaleAdjustment);
                vertices[i] = rotationAdjustment * vertices[i];
            }

            normalizedMesh.vertices = vertices;

            var normals = normalizedMesh.normals;

            for (int i = 0; i < normals.Length; i++)
                normals[i] = rotationAdjustment * normals[i];

            normalizedMesh.normals = normals;

            normalizedMesh.RecalculateBounds();
            normalizedMesh.RecalculateTangents();

            return normalizedMesh;
        }

		public static Vector2 MakeUVs(Vector2 uv, float point, int splineCount, VectorAxis uvAxis, float[] uvResolutions)
		{
			if (uvResolutions.Length == 0)
			{
				Debug.LogError("The UV resolution array is empty");
				return Vector2.zero;
			}

			switch (uvAxis)
			{
				case VectorAxis.X:
					return new Vector2(point * uvResolutions[splineCount], uv.y);
				default:
					return new Vector2(uv.x, point * uvResolutions[splineCount]);
			}
		}

		public static Vector3 GetRequiredOffset(Vector3 vector, VectorAxis axis)
		{
			switch (axis)
			{
				case VectorAxis.X:
					return new Vector3(vector.y, vector.z, 0f);

				default:
				case VectorAxis.Y:
					return new Vector3(vector.x, vector.z, 0f);
			}
		}

		public static float GetRequiredAxis(Vector3 vector, VectorAxis axis)
		{
			switch (axis)
			{
				case VectorAxis.X:
					return vector.x;

				default:
				case VectorAxis.Y:
					return vector.y;
			}
		}
    }
}
