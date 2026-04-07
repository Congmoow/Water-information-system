using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterInfoSystem.Application.Contracts.Monitoring;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/monitoring")]
public class MonitoringController : ControllerBase
{
    private readonly IMonitoringService _monitoringService;

    public MonitoringController(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<MonitoringListItemDto>>>> Search([FromQuery] MonitoringQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _monitoringService.SearchAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<MonitoringListItemDto>>.Success(result));
    }

    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MonitoringTrendPointDto>>>> History([FromQuery] MonitoringQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _monitoringService.GetHistoryAsync(query, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<MonitoringTrendPointDto>>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<MonitoringCreateResultDto>>> Create([FromBody] MonitoringCreateDto request, CancellationToken cancellationToken)
    {
        var result = await _monitoringService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<MonitoringCreateResultDto>.Success(result));
    }
}
