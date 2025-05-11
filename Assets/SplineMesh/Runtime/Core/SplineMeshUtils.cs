using UnityEngine;

namespace SplineMesh.SplineMesh.Runtime.Core
{
	public static class SplineMeshUtils
    {
		public static Vector2 MakeUVs(Vector2 uv, float point, VectorAxis uvAxis, float uvResolutions)
		{
			switch (uvAxis)
			{
				case VectorAxis.X:
					return new Vector2(point * uvResolutions, uv.y);
				default:
					return new Vector2(uv.x, point * uvResolutions);
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
