using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterInfoSystem.Application.Contracts.Alarms;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Shared.Exceptions;
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
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var handledByUserId))
        {
            throw new UnauthorizedException("登录信息无效");
        }

        var result = await _alarmService.HandleAsync(id, request, handledByUserId, cancellationToken);
        return Ok(ApiResponse<AlarmDetailDto>.Success(result));
    }
}
