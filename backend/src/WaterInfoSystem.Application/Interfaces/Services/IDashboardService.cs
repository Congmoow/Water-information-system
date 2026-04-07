using WaterInfoSystem.Application.Contracts.Dashboard;

namespace WaterInfoSystem.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken);
}
