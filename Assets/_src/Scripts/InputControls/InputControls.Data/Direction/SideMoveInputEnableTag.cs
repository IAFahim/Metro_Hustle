using System;
using Unity.Entities;

namespace _src.Scripts.InputControls.InputControls.Data.Direction
{
    public struct SideMoveInputEnableTag : IComponentData, IEnableableComponent
    {
        public bool Live;
        public bool IsRight;
    }
}