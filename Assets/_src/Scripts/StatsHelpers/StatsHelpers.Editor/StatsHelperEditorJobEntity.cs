// #if ALINE
// using _src.Scripts.StatsHelpers.StatsHelpers.Data;
// using BovineLabs.Core;
// using BovineLabs.Stats.Data;
// using Unity.Collections;
// using Unity.Entities;
//
// namespace _src.Scripts.StatsHelpers.StatsHelpers.Editor
// {
//     public partial struct StatsHelperEditorJobEntity : IJobEntity
//     {
//         public Drawing.CommandBuilder Drawing;
//         [ReadOnly] public BufferLookup<Stat> StatsBuffer;
//
//         private void Execute(Entity entity)
//         {
//             if (!StatsBuffer.HasBuffer(entity)) return;
//             foreach (var stat in StatsBuffer[entity].AsMap())
//             {
//                 var statKey = (EStat)stat.Key.Value;
//
//                 BLDebug.LogDebug($"stat {statKey.ToName()} stat.Value: {stat.Value.Value}");
//             }
//         }
//     }
// }
// #endif