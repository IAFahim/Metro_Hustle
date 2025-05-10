using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace ECSSplines.Runtime
{
    /// <summary>
    /// MonoBehaviour component to trigger the baking of an entire SplineContainer
    /// into an ECS NativeSplineContainerBlob Asset. Attach this to the same GameObject
    /// as the SplineContainer you want to bake.
    /// </summary>
    [RequireComponent(typeof(SplineContainer))]
    [DisallowMultipleComponent]
    public class SplineContainerAuthoring : MonoBehaviour
    {
        [Tooltip("If true, pre-calculates and caches up-vectors in the Blob Asset for faster runtime lookup. Increases bake time and blob size.")]
        public bool cacheUpVectors = true;

        [Tooltip("Determines the space the spline geometry is baked in. Local is usually more flexible for moving objects.")]
        public SpaceBakingMode bakingSpace = SpaceBakingMode.LocalSpace;

        public enum SpaceBakingMode
        {
            LocalSpace,
            WorldSpace
        }

        private class SplineContainerBaker : Baker<SplineContainerAuthoring>
        {
            public override void Bake(SplineContainerAuthoring authoring)
            {
                var splineContainer = GetComponent<SplineContainer>();

                if (splineContainer == null || splineContainer.Splines == null || splineContainer.Splines.Count == 0)
                {
                    Debug.LogWarning($"SplineContainerAuthoring on GameObject '{authoring.name}' has no SplineContainer or splines to bake.", authoring);
                    return;
                }

                bool hasValidSpline = false;
                foreach(var spline in splineContainer.Splines) {
                    if (spline != null && spline.Count >= 2) {
                        hasValidSpline = true;
                        break;
                    }
                }
                if (!hasValidSpline) {
                     Debug.LogWarning($"SplineContainerAuthoring on GameObject '{authoring.name}' contains no splines with at least 2 knots.", authoring);
                     return;
                }


                float4x4 bakingTransform = authoring.bakingSpace == SpaceBakingMode.LocalSpace
                    ? float4x4.identity
                    : (float4x4)authoring.transform.localToWorldMatrix;

                BlobAssetReference<NativeSplineContainerBlob> blobRef = default;
                try
                {
                    blobRef = NativeSplineContainerBlobFactory.CreateBlob(
                        splineContainer, bakingTransform,
                        authoring.cacheUpVectors);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error baking spline container '{authoring.name}': {e.Message}\n{e.StackTrace}", authoring);
                    return;
                }

                if (!blobRef.IsCreated)
                {
                     Debug.LogError($"Failed to create spline container blob for '{authoring.name}'. Check previous warnings.", authoring);
                     return;
                }

                var transformUsage = authoring.bakingSpace == SpaceBakingMode.LocalSpace
                    ? TransformUsageFlags.Dynamic
                    : TransformUsageFlags.Renderable;
                var entity = GetEntity(transformUsage);

                AddComponent(entity, new SplineContainerBlobComponent
                {
                    Value = blobRef
                });

                AddBlobAsset(ref blobRef, out _);
            }
        }
    }
}