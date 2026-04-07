using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterInfoSystem.Application.Contracts.Stations;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/station")]
public class StationController : ControllerBase
{
    private readonly IStationService _stationService;

    public StationController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<StationListItemDto>>>> Search([FromQuery] StationQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _stationService.SearchAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<StationListItemDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<StationDetailDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _stationService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<StationDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StationDetailDto>>> Create([FromBody] StationUpsertDto request, CancellationToken cancellationToken)
    {
        var result = await _stationService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<StationDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<StationDetailDto>>> Update(Guid id, [FromBody] StationUpsertDto request, CancellationToken cancellationToken)
    {
        var result = await _stationService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<StationDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _stationService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "删除成功"));
    }
}
