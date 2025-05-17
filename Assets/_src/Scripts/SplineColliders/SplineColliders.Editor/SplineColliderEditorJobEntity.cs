#if ALINE
using Drawing;
using System.ComponentModel;
using _src.Scripts.Colliders.Colliders.Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.SplineColliders.SplineColliders.Editor
{
    public partial struct SplineColliderEditorJobEntity : IJobEntity
    {
        [ReadOnly(true)] public CommandBuilder Drawing;

        private void Execute(in LocalToWorld localToWorld,
            in ColliderHeightComponent colliderHeightComponent,
            in ColliderRadiusSqComponent colliderRadiusSqComponent
        )
        {
            var up = localToWorld.Up * colliderHeightComponent.Height;
            Drawing.WireSphere(localToWorld.Position + up, math.sqrt(colliderRadiusSqComponent.RadiusSq));
        }
    }
}
#endif