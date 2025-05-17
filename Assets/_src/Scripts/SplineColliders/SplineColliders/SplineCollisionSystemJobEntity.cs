using _src.Scripts.SplineColliders.SplineColliders.Data;
using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders
{
    [WithAbsent(typeof(SplineMainColliderTag))]
    public partial struct SplineCollisionSystemJobEntity : IJobEntity
    {
        [ReadOnly] public NativeArray<ColliderEntityData>.ReadOnly MainColliders;

        private void Execute(in SplineLineComponent splineLineComponent)
        {
            for (var i = 0; i < MainColliders.Length; i++)
            {
                var main = MainColliders[i];
                if (splineLineComponent.SplineLine != main.SplineLineComponent.SplineLine) continue;
                Debug.Log("Collided");
            }
        }
    }
}