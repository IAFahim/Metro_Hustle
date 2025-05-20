using System;
using _src.Scripts.InputControls.InputControls.Data.Direction;
using _src.Scripts.SplineConfigs.SplineConfigs.Data;
using Unity.Entities;

namespace _src.Scripts.InputConsumers.InputConsumers
{
    public partial struct InputConsumerJobEntity : IJobEntity
    {
        public void Execute(ref DirectionInputComponent directionInputComponent ,ref SplineLineComponent splineLineComponent)
        {
            switch (directionInputComponent.DirectionFlag)
            {
                case DirectionFlag.None:
                    break;
                case DirectionFlag.LeftEnable:
                    break;
                case DirectionFlag.UpEnable:
                    break;
                case DirectionFlag.DownEnable:
                    break;
                case DirectionFlag.RightEnable:
                    break;
                case DirectionFlag.IsLeft:
                    break;
                case DirectionFlag.IsUp:
                    break;
                case DirectionFlag.IsDown:
                    break;
                case DirectionFlag.IsRight:
                    break;
                case DirectionFlag.IsRightEnabledAndActive:
                    break;
                case DirectionFlag.IsUpEnabledAndActive:
                    break;
                case DirectionFlag.IsDownEnabledAndActive:
                    break;
                case DirectionFlag.IsLeftEnabledAndActive:
                    break;
                case DirectionFlag.ActiveStateFlagsMask:
                    break;
                case DirectionFlag.EnableFlagsMask:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}