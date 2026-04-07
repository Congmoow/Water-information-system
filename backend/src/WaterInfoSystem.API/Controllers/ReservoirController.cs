using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterInfoSystem.Application.Contracts.Reservoirs;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/reservoir")]
public class ReservoirController : ControllerBase
{
    private readonly IReservoirService _reservoirService;

    public ReservoirController(IReservoirService reservoirService)
    {
        _reservoirService = reservoirService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ReservoirListItemDto>>>> Search([FromQuery] ReservoirQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _reservoirService.SearchAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<ReservoirListItemDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ReservoirDetailDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reservoirService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<ReservoirDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReservoirDetailDto>>> Create([FromBody] ReservoirUpsertDto request, CancellationToken cancellationToken)
    {
        var result = await _reservoirService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<ReservoirDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ReservoirDetailDto>>> Update(Guid id, [FromBody] ReservoirUpsertDto request, CancellationToken cancellationToken)
    {
        var result = await _reservoirService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<ReservoirDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _reservoirService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "删除成功"));
    }
}
