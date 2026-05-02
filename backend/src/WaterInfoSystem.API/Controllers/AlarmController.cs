using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterInfoSystem.Application.Contracts.Alarms;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/alarm")]
public class AlarmController : ControllerBase
{
    private readonly IAlarmService _alarmService;

    public AlarmController(IAlarmService alarmService)
    {
        _alarmService = alarmService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AlarmListItemDto>>>> Search([FromQuery] AlarmQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _alarmService.SearchAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<AlarmListItemDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<AlarmDetailDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _alarmService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<AlarmDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AlarmDetailDto>>> Create([FromBody] AlarmCreateDto request, CancellationToken cancellationToken)
    {
        var result = await _alarmService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<AlarmDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:guid}/handle")]
    public async Task<ActionResult<ApiResponse<AlarmDetailDto>>> Handle(Guid id, [FromBody] AlarmHandleDto request, CancellationToken cancellationToken)
    {
        var handledByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var payload = request with
        {
            HandledByUserId = Guid.TryParse(handledByUserId, out var parsedUserId)
                ? parsedUserId
                : request.HandledByUserId
        };

        var result = await _alarmService.HandleAsync(id, payload, cancellationToken);
        return Ok(ApiResponse<AlarmDetailDto>.Success(result));
    }
}
