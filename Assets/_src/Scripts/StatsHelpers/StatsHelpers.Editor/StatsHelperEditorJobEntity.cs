#if ALINE
using BovineLabs.Stats.Data;
using Unity.Entities;

namespace _src.Scripts.StatsHelpers.StatsHelpers.Editor
{
    public partial struct StatsHelperEditorJobEntity : IJobEntity
    {
        public Drawing.CommandBuilder Drawing;
        public BufferLookup<Stat> StatsBuffer;
        private void Execute(Entity entity)
        {
            
            var enumerator = StatsBuffer[entity].ElementAt(0);
        }
    }
}
#endif