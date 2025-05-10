#if UNITY_EDITOR
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _src.Scripts.SplineMovement.Runtime.Datas
{
    public class SplineLinkComponentDataAuthoring : MonoBehaviour
    {
        public TransformUsageFlags transformUsageFlags = TransformUsageFlags.Dynamic;
        public byte splineIndex;
        public byte curveIndex;
        public sbyte lineNumber;
        public float distanceInCurve;
        public float distanceOffset;


        public class SplineLinkComponentDataBaker : Baker<SplineLinkComponentDataAuthoring>
        {
            public override void Bake(SplineLinkComponentDataAuthoring authoring)
            {
                var entity = GetEntity(authoring.transformUsageFlags);
                AddComponent(entity, new SplineLinkComponentData
                    {
                        SplineIndex = authoring.splineIndex,
                        CurveIndex = authoring.curveIndex,
                        LineNumber = authoring.lineNumber,
                        DistanceInCurve = authoring.distanceInCurve,
                        DistanceOffset = authoring.distanceOffset,
                    }
                );

                AddComponent(entity, new SplineEntityLocationComponentData()
                {
                    Position = 0,
                    LookRotationSafe = new quaternion()
                });
            }
        }
    }
}
#endif