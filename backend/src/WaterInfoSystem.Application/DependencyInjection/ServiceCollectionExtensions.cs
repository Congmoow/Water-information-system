using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Application.Services;

namespace WaterInfoSystem.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReservoirService, ReservoirService>();
        services.AddScoped<IRiverService, RiverService>();
        services.AddScoped<IStationService, StationService>();
        services.AddScoped<IMonitoringService, MonitoringService>();
        services.AddScoped<IAlarmService, AlarmService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IMapService, MapService>();
        services.AddScoped<IApprovalService, ApprovalService>();
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        return services;
    }
}
