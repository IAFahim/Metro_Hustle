#if ALINE
using Drawing;
using System.ComponentModel;
using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Transforms;

namespace _src.Scripts.SplineColliders.SplineColliders.Editor
{
    public partial struct SplineColliderEditorJobEntity : IJobEntity
    {
        [ReadOnly(true)] public CommandBuilder Drawing;

        private void Execute(in LocalToWorld localToWorld,
            in ColliderHeightComponent colliderHeightComponent,
            in ColliderRadiusComponent colliderRadiusComponent
        )
        {
            var up = localToWorld.Up * colliderHeightComponent.Height;
            Drawing.WireSphere(localToWorld.Position + up, colliderRadiusComponent.Radius);
        }
    }
}
#endif