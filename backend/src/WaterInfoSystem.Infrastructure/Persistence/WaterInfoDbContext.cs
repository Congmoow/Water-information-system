using Microsoft.EntityFrameworkCore;
using WaterInfoSystem.Application.Interfaces;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Infrastructure.Persistence;

public class WaterInfoDbContext : DbContext, IUnitOfWork
{
    public WaterInfoDbContext(DbContextOptions<WaterInfoDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Reservoir> Reservoirs => Set<Reservoir>();

    public DbSet<River> Rivers => Set<River>();

    public DbSet<Station> Stations => Set<Station>();

    public DbSet<MonitoringData> MonitoringDatas => Set<MonitoringData>();

    public DbSet<AlarmRecord> AlarmRecords => Set<AlarmRecord>();

    public DbSet<ApprovalApplication> ApprovalApplications => Set<ApprovalApplication>();

    public DbSet<ApprovalAttachment> ApprovalAttachments => Set<ApprovalAttachment>();

    public DbSet<ReviewResult> ReviewResults => Set<ReviewResult>();

    public DbSet<ReviewFinding> ReviewFindings => Set<ReviewFinding>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WaterInfoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return SaveChangesAsync(cancellationToken);
    }
}
