using WaterInfoSystem.Application.Contracts.Maps;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Application.Interfaces.Services;

namespace WaterInfoSystem.Application.Services;

public class MapService : IMapService
{
    private readonly IReservoirRepository _reservoirRepository;
    private readonly IRiverRepository _riverRepository;
    private readonly IStationRepository _stationRepository;

    public MapService(
        IReservoirRepository reservoirRepository,
        IRiverRepository riverRepository,
        IStationRepository stationRepository)
    {
        _reservoirRepository = reservoirRepository;
        _riverRepository = riverRepository;
        _stationRepository = stationRepository;
    }

    public async Task<MapDataDto> GetMapDataAsync(CancellationToken cancellationToken)
    {
        var reservoirs = await _reservoirRepository.GetAllAsync(cancellationToken);
        var rivers = await _riverRepository.GetAllAsync(cancellationToken);
        var stations = await _stationRepository.GetAllAsync(cancellationToken);

        var items = reservoirs
            .Select(x => new MapPointDto(x.Id, x.Name, "Reservoir", x.Latitude, x.Longitude, x.Description, null, "reservoir"))
            .Concat(rivers.Select(x => new MapPointDto(x.Id, x.Name, "River", x.Latitude, x.Longitude, x.Description, null, "river")))
            .Concat(stations.Select(x => new MapPointDto(x.Id, x.Name, x.Type.ToString(), x.Latitude, x.Longitude, x.Description, x.Status.ToString(), "station")))
            .ToList();

        return new MapDataDto(items);
    }
}
