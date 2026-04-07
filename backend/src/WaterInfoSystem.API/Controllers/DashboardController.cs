using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterInfoSystem.Application.Contracts.Dashboard;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("overview")]
    public async Task<ActionResult<ApiResponse<DashboardOverviewDto>>> GetOverview(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetOverviewAsync(cancellationToken);
        return Ok(ApiResponse<DashboardOverviewDto>.Success(result));
    }
}
