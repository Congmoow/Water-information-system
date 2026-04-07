using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterInfoSystem.Application.Contracts.Maps;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/map")]
public class MapController : ControllerBase
{
    private readonly IMapService _mapService;

    public MapController(IMapService mapService)
    {
        _mapService = mapService;
    }

    [HttpGet("points")]
    public async Task<ActionResult<ApiResponse<MapDataDto>>> GetPoints(CancellationToken cancellationToken)
    {
        var result = await _mapService.GetMapDataAsync(cancellationToken);
        return Ok(ApiResponse<MapDataDto>.Success(result));
    }
}
