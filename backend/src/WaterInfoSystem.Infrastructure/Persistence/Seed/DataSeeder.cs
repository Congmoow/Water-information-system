using Microsoft.EntityFrameworkCore;
using WaterInfoSystem.Application.Interfaces.Security;
using WaterInfoSystem.Domain.Constants;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Infrastructure.Persistence.Seed;

public class DataSeeder
{
    private readonly WaterInfoDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public DataSeeder(WaterInfoDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // 优先使用 MigrateAsync 以保持迁移历史可追溯。
        // 若数据库此前由 EnsureCreated 创建（无迁移历史表），回退 EnsureCreated 兼容。
        try
        {
            await _dbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("__EFMigrationsHistory"))
        {
            // 迁移历史表不存在时回退到 EnsureCreated，其他异常（连接超时、认证失败等）正常抛出。
            await _dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }

        if (await _dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.Now;

        var users = new[]
        {
            new User
            {
                Id = DemoDataIds.AdminUserId,
                Username = "admin",
                PasswordHash = _passwordHasher.HashPassword("Admin@123"),
                FullName = "张伟",
                Role = UserRole.Administrator,
                CreatedAt = now,
                UpdatedAt = now
            },
            new User
            {
                Id = DemoDataIds.ViewerUserId,
                Username = "viewer",
                PasswordHash = _passwordHasher.HashPassword("Viewer@123"),
                FullName = "李敏",
                Role = UserRole.User,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        var reservoirs = new[]
        {
            new Reservoir
            {
                Id = DemoDataIds.EastLakeReservoirId,
                Name = "东湖水库",
                Location = "江州市东湖新区",
                Capacity = 1260.5m,
                ManagementUnit = "江州市水务局",
                Latitude = 30.625,
                Longitude = 114.347,
                Description = "承担城市供水与防汛调蓄任务。",
                CreatedAt = now,
                UpdatedAt = now
            },
            new Reservoir
            {
                Id = DemoDataIds.WestHillReservoirId,
                Name = "西山水库",
                Location = "江州市西山镇",
                Capacity = 980.2m,
                ManagementUnit = "西山区水利站",
                Latitude = 30.576,
                Longitude = 114.287,
                Description = "主要服务西山片区灌溉与生态补水。",
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        var rivers = new[]
        {
            new River
            {
                Id = DemoDataIds.SouthCreekRiverId,
                Name = "南溪河",
                Length = 68.5m,
                Basin = "南溪流域",
                Latitude = 30.595,
                Longitude = 114.398,
                Description = "区域内重要中小河道，汛期雨量变化明显。",
                CreatedAt = now,
                UpdatedAt = now
            },
            new River
            {
                Id = DemoDataIds.DragonBayRiverId,
                Name = "龙湾河",
                Length = 43.2m,
                Basin = "龙湾流域",
                Latitude = 30.649,
                Longitude = 114.271,
                Description = "承担沿线排涝与生态补水任务。",
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        var stations = new[]
        {
            new Station
            {
                Id = DemoDataIds.EastLakeWaterLevelStationId,
                Name = "东湖1号水位站",
                Type = StationType.WaterLevel,
                Longitude = 114.349,
                Latitude = 30.626,
                Status = StationStatus.Online,
                WarningThreshold = SystemDefaults.DefaultWaterLevelWarningThreshold,
                CriticalThreshold = SystemDefaults.DefaultWaterLevelCriticalThreshold,
                Description = "东湖水库主坝前水位站。",
                LastActiveAt = now.AddMinutes(-5),
                ReservoirId = DemoDataIds.EastLakeReservoirId,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Station
            {
                Id = DemoDataIds.SouthCreekRainStationId,
                Name = "南溪河雨量站",
                Type = StationType.Rainfall,
                Longitude = 114.401,
                Latitude = 30.593,
                Status = StationStatus.Online,
                WarningThreshold = SystemDefaults.DefaultRainfallWarningThreshold,
                CriticalThreshold = SystemDefaults.DefaultRainfallCriticalThreshold,
                Description = "南溪河流域自动雨量监测站。",
                LastActiveAt = now.AddMinutes(-12),
                RiverId = DemoDataIds.SouthCreekRiverId,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Station
            {
                Id = DemoDataIds.DragonBayFlowStationId,
                Name = "龙湾流量站",
                Type = StationType.Flow,
                Longitude = 114.274,
                Latitude = 30.648,
                Status = StationStatus.Warning,
                WarningThreshold = SystemDefaults.DefaultFlowWarningThreshold,
                CriticalThreshold = SystemDefaults.DefaultFlowCriticalThreshold,
                Description = "龙湾河下游流量监测站。",
                LastActiveAt = now.AddMinutes(-35),
                RiverId = DemoDataIds.DragonBayRiverId,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Station
            {
                Id = DemoDataIds.WestHillBackupStationId,
                Name = "西山2号水位站",
                Type = StationType.WaterLevel,
                Longitude = 114.291,
                Latitude = 30.579,
                Status = StationStatus.Offline,
                WarningThreshold = SystemDefaults.DefaultWaterLevelWarningThreshold,
                CriticalThreshold = SystemDefaults.DefaultWaterLevelCriticalThreshold,
                Description = "西山水库备用站，当前离线。",
                LastActiveAt = now.AddMinutes(-48),
                ReservoirId = DemoDataIds.WestHillReservoirId,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        var monitoringDatas = BuildMonitoringDatas(now);
        var alarms = BuildAlarmRecords(now, monitoringDatas);

        await _dbContext.Users.AddRangeAsync(users, cancellationToken);
        await _dbContext.Reservoirs.AddRangeAsync(reservoirs, cancellationToken);
        await _dbContext.Rivers.AddRangeAsync(rivers, cancellationToken);
        await _dbContext.Stations.AddRangeAsync(stations, cancellationToken);
        await _dbContext.MonitoringDatas.AddRangeAsync(monitoringDatas, cancellationToken);
        await _dbContext.AlarmRecords.AddRangeAsync(alarms, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IReadOnlyList<MonitoringData> BuildMonitoringDatas(DateTime now)
    {
        var items = new List<MonitoringData>();

        for (var dayOffset = 6; dayOffset >= 0; dayOffset--)
        {
            var collectedAt = now.Date.AddDays(-dayOffset).AddHours(9);

            items.Add(new MonitoringData
            {
                Id = Guid.NewGuid(),
                StationId = DemoDataIds.EastLakeWaterLevelStationId,
                DataType = MonitoringDataType.WaterLevel,
                Value = 16.2m + (6 - dayOffset) * 0.55m,
                CollectedAt = collectedAt,
                Remark = "水位自动采集",
                CreatedAt = collectedAt,
                UpdatedAt = collectedAt
            });

            items.Add(new MonitoringData
            {
                Id = Guid.NewGuid(),
                StationId = DemoDataIds.SouthCreekRainStationId,
                DataType = MonitoringDataType.Rainfall,
                Value = 18m + (6 - dayOffset) * 8m,
                CollectedAt = collectedAt.AddHours(1),
                Remark = "雨量自动采集",
                CreatedAt = collectedAt.AddHours(1),
                UpdatedAt = collectedAt.AddHours(1)
            });

            items.Add(new MonitoringData
            {
                Id = Guid.NewGuid(),
                StationId = DemoDataIds.DragonBayFlowStationId,
                DataType = MonitoringDataType.Flow,
                Value = 150m + (6 - dayOffset) * 28m,
                CollectedAt = collectedAt.AddHours(2),
                Remark = "流量自动采集",
                CreatedAt = collectedAt.AddHours(2),
                UpdatedAt = collectedAt.AddHours(2)
            });
        }

        return items;
    }

    private static IReadOnlyList<AlarmRecord> BuildAlarmRecords(DateTime now, IReadOnlyList<MonitoringData> monitoringDatas)
    {
        var latestWaterLevel = monitoringDatas
            .Where(x => x.StationId == DemoDataIds.EastLakeWaterLevelStationId)
            .MaxBy(x => x.CollectedAt)!;

        var latestRainfall = monitoringDatas
            .Where(x => x.StationId == DemoDataIds.SouthCreekRainStationId)
            .MaxBy(x => x.CollectedAt)!;

        var latestFlow = monitoringDatas
            .Where(x => x.StationId == DemoDataIds.DragonBayFlowStationId)
            .MaxBy(x => x.CollectedAt)!;

        return new[]
        {
            new AlarmRecord
            {
                Id = Guid.NewGuid(),
                StationId = DemoDataIds.EastLakeWaterLevelStationId,
                MonitoringDataId = latestWaterLevel.Id,
                AlarmType = AlarmType.ThresholdExceeded,
                Level = AlarmLevel.Warning,
                Status = AlarmStatus.Pending,
                Message = "东湖1号水位站水位超过警戒值",
                TriggeredAt = latestWaterLevel.CollectedAt,
                CreatedAt = latestWaterLevel.CollectedAt,
                UpdatedAt = latestWaterLevel.CollectedAt
            },
            new AlarmRecord
            {
                Id = Guid.NewGuid(),
                StationId = DemoDataIds.SouthCreekRainStationId,
                MonitoringDataId = latestRainfall.Id,
                AlarmType = AlarmType.ThresholdExceeded,
                Level = AlarmLevel.Critical,
                Status = AlarmStatus.Processing,
                Message = "南溪河雨量站雨量超过严重阈值",
                TriggeredAt = latestRainfall.CollectedAt,
                HandleRemark = "已通知值班人员加强巡查",
                HandledAt = latestRainfall.CollectedAt.AddMinutes(20),
                HandledByUserId = DemoDataIds.AdminUserId,
                CreatedAt = latestRainfall.CollectedAt,
                UpdatedAt = latestRainfall.CollectedAt.AddMinutes(20)
            },
            new AlarmRecord
            {
                Id = Guid.NewGuid(),
                StationId = DemoDataIds.DragonBayFlowStationId,
                MonitoringDataId = latestFlow.Id,
                AlarmType = AlarmType.ThresholdExceeded,
                Level = AlarmLevel.Warning,
                Status = AlarmStatus.Pending,
                Message = "龙湾流量站流量持续高位运行",
                TriggeredAt = latestFlow.CollectedAt,
                CreatedAt = latestFlow.CollectedAt,
                UpdatedAt = latestFlow.CollectedAt
            },
            new AlarmRecord
            {
                Id = Guid.NewGuid(),
                StationId = DemoDataIds.WestHillBackupStationId,
                AlarmType = AlarmType.StationOffline,
                Level = AlarmLevel.Warning,
                Status = AlarmStatus.Pending,
                Message = "站点离线超过30分钟",
                TriggeredAt = now.AddMinutes(-30),
                CreatedAt = now.AddMinutes(-30),
                UpdatedAt = now.AddMinutes(-30)
            }
        };
    }
}
