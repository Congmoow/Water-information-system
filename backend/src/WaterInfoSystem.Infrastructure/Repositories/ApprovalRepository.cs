using Microsoft.EntityFrameworkCore;
using WaterInfoSystem.Application.Contracts.Approvals;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Infrastructure.Persistence;

namespace WaterInfoSystem.Infrastructure.Repositories;

public class ApprovalRepository : IApprovalRepository
{
    private readonly WaterInfoDbContext _dbContext;

    public ApprovalRepository(WaterInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(List<ApprovalListItemDto> Items, int Total)> SearchAsync(
        string? keyword, string? status, int page, int pageSize, CancellationToken cancellationToken)
    {
        var q = _dbContext.ApprovalApplications.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            q = q.Where(a => a.ApplicantName.Contains(keyword)
                           || a.EnterpriseName!.Contains(keyword)
                           || a.WaterIntakeLocation.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            q = q.Where(a => a.Status == status);
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(a => a.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ApprovalListItemDto(
                a.Id,
                a.ApplicantName,
                a.ApplicantIdCard,
                a.EnterpriseName,
                a.WaterIntakeLocation,
                a.WaterIntakePurpose,
                a.WaterIntakeAmount,
                a.ApplicationDate,
                a.Status,
                a.UpdatedAt))
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<ApprovalApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.ApprovalApplications
            .Include(a => a.Attachments)
            .Include(a => a.ReviewResults)
                .ThenInclude(r => r.Findings)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public Task AddAsync(ApprovalApplication entity, CancellationToken cancellationToken)
    {
        _dbContext.ApprovalApplications.Add(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ApprovalApplication entity, CancellationToken cancellationToken)
    {
        _dbContext.ApprovalApplications.Remove(entity);
        return Task.CompletedTask;
    }
}
