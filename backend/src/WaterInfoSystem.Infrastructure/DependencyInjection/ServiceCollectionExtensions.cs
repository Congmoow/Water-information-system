using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WaterInfoSystem.Application.Interfaces;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Application.Interfaces.Security;
using WaterInfoSystem.Infrastructure.Identity;
using WaterInfoSystem.Infrastructure.Persistence;
using WaterInfoSystem.Infrastructure.Persistence.Seed;
using WaterInfoSystem.Infrastructure.Repositories;

namespace WaterInfoSystem.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddDbContext<WaterInfoDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WaterInfoDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReservoirRepository, ReservoirRepository>();
        services.AddScoped<IRiverRepository, RiverRepository>();
        services.AddScoped<IStationRepository, StationRepository>();
        services.AddScoped<IMonitoringRepository, MonitoringRepository>();
        services.AddScoped<IAlarmRepository, AlarmRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<DataSeeder>();

        return services;
    }
}
