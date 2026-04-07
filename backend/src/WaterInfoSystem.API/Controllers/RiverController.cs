using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterInfoSystem.Application.Contracts.Rivers;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/river")]
public class RiverController : ControllerBase
{
    private readonly IRiverService _riverService;

    public RiverController(IRiverService riverService)
    {
        _riverService = riverService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<RiverListItemDto>>>> Search([FromQuery] RiverQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _riverService.SearchAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<RiverListItemDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RiverDetailDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _riverService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<RiverDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RiverDetailDto>>> Create([FromBody] RiverUpsertDto request, CancellationToken cancellationToken)
    {
        var result = await _riverService.CreateAsync(request, cancellationToken);
        return Ok(ApiResponse<RiverDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<RiverDetailDto>>> Update(Guid id, [FromBody] RiverUpsertDto request, CancellationToken cancellationToken)
    {
        var result = await _riverService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<RiverDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _riverService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "删除成功"));
    }
}
