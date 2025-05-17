#if ALINE
using Drawing;
using System.ComponentModel;
using _src.Scripts.SplineColliders.SplineColliders.Data;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.SplineColliders.SplineColliders.Editor
{
    public partial struct SplineColliderEditorJobEntity : IJobEntity
    {
        [ReadOnly(true)] public CommandBuilder Drawing;

        private void Execute(in LocalTransform localTransform,
            in SplineColliderHeightComponent splineColliderHeightComponent,
            in SplineColliderRadiusComponent splineColliderRadiusComponent
        )
        {
            var up = localTransform.Up() * splineColliderHeightComponent.Height;
            Drawing.WireSphere(localTransform.Position + up, splineColliderRadiusComponent.Radius);
        }
    }
}
#endif