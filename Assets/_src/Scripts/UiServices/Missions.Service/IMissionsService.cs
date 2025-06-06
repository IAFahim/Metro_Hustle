using _src.Scripts.Missions.Missions.Data;

namespace _src.Scripts.UiServices.Missions.Service
{
    public interface IMissionsService
    {
        AssetReferenceMissions GetAsset(ushort index);
        AssetReferenceMissions GetCurrent();
    }
}