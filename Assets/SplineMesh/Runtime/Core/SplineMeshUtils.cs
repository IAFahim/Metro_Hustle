using UnityEngine;

namespace SplineMesh.SplineMesh.Runtime.Core
{
	public static class SplineMeshUtils
    {
		public static Vector2 MakeUVs(Vector2 uv, float point, VectorAxis uvAxis, float uvResolutions)
		{
			return uvAxis switch
			{
				VectorAxis.X => new Vector2(point * uvResolutions, uv.y),
				_ => new Vector2(uv.x, point * uvResolutions)
			};
		}

		public static Vector3 GetRequiredOffset(Vector3 vector, VectorAxis axis)
		{
			return axis switch
			{
				VectorAxis.X => new Vector3(vector.y, vector.z, 0f),
				VectorAxis.Y => new Vector3(vector.x, vector.z, 0f),
				_ => new Vector3(vector.x, vector.z, 0f)
			};
		}

		public static float GetRequiredAxis(Vector3 vector, VectorAxis axis)
		{
			return axis switch
			{
				VectorAxis.X => vector.x,
				VectorAxis.Y => vector.y,
				_ => vector.y
			};
		}
    }
}
