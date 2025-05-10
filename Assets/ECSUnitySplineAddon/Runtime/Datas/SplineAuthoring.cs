using ECS_Spline.Runtime.Datas;
using ECSUnitySplineAddon.Runtime.Datas;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace ECSSplines.Runtime
{
    /// <summary>
    /// MonoBehaviour component to trigger the baking of a SplineContainer into an ECS Blob Asset.
    /// </summary>
    [RequireComponent(typeof(SplineContainer))]
    public class SplineAuthoring : MonoBehaviour
    {
        [Tooltip("If true, pre-calculates and caches up-vectors in the Blob Asset for faster runtime lookup. Increases bake time and blob size.")]
        public bool cacheUpVectors = true;

        [Tooltip("Determines the space the spline is baked in. Local is usually more flexible for moving objects.")]
        public SpaceBakingMode bakingSpace = SpaceBakingMode.LocalSpace;

        public enum SpaceBakingMode
        {
            LocalSpace,
            WorldSpace
        }

        private class SplineBaker : Baker<SplineAuthoring>
        {
            public override void Bake(SplineAuthoring authoring)
            {
                var splineContainer = GetComponent<SplineContainer>();

                if (splineContainer == null || splineContainer.Splines == null || splineContainer.Splines.Count == 0)
                {
                    Debug.LogWarning($"SplineAuthoring on GameObject '{authoring.name}' has no SplineContainer or splines to bake.", authoring);
                    return;
                }

                float4x4 bakingTransform = authoring.bakingSpace == SpaceBakingMode.LocalSpace
                    ? float4x4.identity
                    : (float4x4)authoring.transform.localToWorldMatrix;


                Spline splineToBake = splineContainer.Splines[0];

                if (splineToBake == null || splineToBake.Count < 2)
                {
                     Debug.LogWarning($"Spline at index 0 in SplineContainer on '{authoring.name}' is null or has less than 2 knots. Skipping bake for this spline.", authoring);
                     return;
                }


                BlobAssetReference<NativeSplineBlob> blobRef;
                try
                {
                    blobRef = NativeSplineBlobFactory.CreateBlob(
                         splineToBake,
                         bakingTransform,
                         authoring.cacheUpVectors,
                         Allocator.Persistent);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error baking spline '{authoring.name}': {e.Message}\n{e.StackTrace}", authoring);
                    return;
                }


                var transformUsage = authoring.bakingSpace == SpaceBakingMode.LocalSpace ? TransformUsageFlags.Dynamic : TransformUsageFlags.Renderable;
                var entity = GetEntity(transformUsage);

                AddComponent(entity, new NativeSplineBlobComponentData
                {
                    Value = blobRef
                });

                AddBlobAsset(ref blobRef, out _);
            }
        }
    }
}