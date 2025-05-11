﻿using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Splines;

namespace ECSUnitySplineAddon.Runtime.Datas
{
    [RequireComponent(typeof(SplineContainer))]
    public class SplineComponentAuthoring : MonoBehaviour
    {
        public SplineContainer splineContainer;
        public int LUT_RESOLUTION = 30;

        private void Reset()
        {
            splineContainer = GetComponent<SplineContainer>();
        }

        public class SplineComponentBaker : Baker<SplineComponentAuthoring>
        {
            public override void Bake(SplineComponentAuthoring authoring)
            {
                var splineContainer = authoring.splineContainer;

                if (splineContainer is null)
                {
                    Debug.Log($"From {nameof(SplineComponentBaker)}.Bake(). spline container is null");
                    return;
                }

                var spline = splineContainer.Spline;
                using var nativeSpline = new NativeSpline(spline);

                var nativeSplineBlobAssetRef =
                    NativeSplineBlobFactory.CreateBlob(nativeSpline, authoring.LUT_RESOLUTION);

                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddBlobAsset(ref nativeSplineBlobAssetRef, out _);

                AddComponent(entity, new NativeSplineBlobComponentData
                {
                    Value = nativeSplineBlobAssetRef
                });
            }
        }
    }
}