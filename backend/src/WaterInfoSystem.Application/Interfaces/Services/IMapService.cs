using WaterInfoSystem.Application.Contracts.Maps;

namespace WaterInfoSystem.Application.Interfaces.Services;

public interface IMapService
{
    Task<MapDataDto> GetMapDataAsync(CancellationToken cancellationToken);
}
