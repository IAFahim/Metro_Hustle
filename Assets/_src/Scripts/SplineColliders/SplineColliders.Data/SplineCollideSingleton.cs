using Unity.Collections;
using Unity.Entities;

namespace _src.Scripts.SplineColliders.SplineColliders.Data
{
    public struct SplineCollideSingleton : IComponentData
    {
        public NativeQueue<int> ag;
    }
}