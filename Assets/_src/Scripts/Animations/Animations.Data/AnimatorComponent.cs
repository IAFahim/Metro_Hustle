using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.Animations.Animations.Data
{
    public struct AnimatorComponent : ICleanupComponentData
    {
        public UnityObjectRef<Animator> Ref;
        public byte CurrentState;
        public byte OldState;
    }
}