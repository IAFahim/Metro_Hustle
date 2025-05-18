using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Entities;

namespace _src.Scripts.SplineConfigs.SplineConfigs.Data
{
    [BurstCompile]
    public struct SplineLineComponent : IComponentData
    {
        public byte Value;

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetSpline() => Value >> 5;

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SplineEncode(byte splineLine) => (byte)(splineLine << 3);

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte SplineLineEncode(byte spline, byte line) => (byte)((spline << 3) | line);

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SplineLineComponent Create(byte spline, byte line) =>
            new() { Value = SplineLineEncode(spline, line) };

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int GetLine() => Value & 0x1111_1000;
    }
}