#if ALINE
using System.ComponentModel;
using _src.Scripts.Colliders.Colliders.Data;
using Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _src.Scripts.Colliders.Colliders.Editor
{
    public partial struct SplineColliderEditorJobEntity : IJobEntity
    {
        [ReadOnly(true)] public CommandBuilder Drawing;

        private void Execute(in LocalToWorld localToWorld,
            in ColliderHeightComponent colliderHeightComponent,
            in ColliderRadiusSqComponent colliderRadiusSqComponent,
            in ColliderPreHitComponent colliderPreHitComponent
        )
        {
            var up = localToWorld.Up * colliderHeightComponent.Up;
            var position = localToWorld.Position;
            Drawing.WireSphere(position + up, math.sqrt(colliderRadiusSqComponent.RadiusSq));
            var forward = localToWorld.Forward * colliderPreHitComponent.Forward;
            Drawing.WireSphere(position + forward, math.sqrt(colliderPreHitComponent.RadiusSq));
        }
    }
}
#endif