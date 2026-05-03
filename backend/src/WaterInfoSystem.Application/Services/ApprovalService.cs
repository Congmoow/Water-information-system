using Microsoft.EntityFrameworkCore;
using WaterInfoSystem.Application.Contracts.Approvals;
using WaterInfoSystem.Application.Interfaces;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Shared.Exceptions;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Services;

public class ApprovalService : IApprovalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiServiceClient _aiClient;

    public ApprovalService(IUnitOfWork unitOfWork, IAiServiceClient aiClient)
    {
        _unitOfWork = unitOfWork;
        _aiClient = aiClient;
    }

    private DbContext Db => (DbContext)_unitOfWork;

    public async Task<PagedResult<ApprovalListItemDto>> SearchAsync(ApprovalQueryDto query, CancellationToken cancellationToken)
    {
        var set = Db.Set<ApprovalApplication>();
        var q = set.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            q = q.Where(a => a.ApplicantName.Contains(query.Keyword)
                           || a.EnterpriseName!.Contains(query.Keyword)
                           || a.WaterIntakeLocation.Contains(query.Keyword));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            q = q.Where(a => a.Status == query.Status);
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(a => a.UpdatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
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

        return new PagedResult<ApprovalListItemDto>(items, total);
    }

    public async Task<ApprovalDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        return MapDetail(entity);
    }

    public async Task<ApprovalDetailDto> CreateAsync(ApprovalCreateDto request, Guid userId, CancellationToken cancellationToken)
    {
        var entity = new ApprovalApplication
        {
            ApplicantName = request.ApplicantName,
            ApplicantIdCard = request.ApplicantIdCard,
            EnterpriseName = request.EnterpriseName,
            EnterpriseLicenseNo = request.EnterpriseLicenseNo,
            WaterIntakeLocation = request.WaterIntakeLocation,
            WaterIntakePurpose = request.WaterIntakePurpose,
            WaterIntakeAmount = request.WaterIntakeAmount,
            ApplicationDate = DateTime.UtcNow,
            Status = "Pending",
            SubmittedByUserId = userId,
        };

        Db.Set<ApprovalApplication>().Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapDetail(entity);
    }

    public async Task<ApprovalDetailDto> SubmitForReviewAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        entity.Status = "Reviewing";
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 调用 Python AI 服务进行审查
        try
        {
            var reviewResult = await _aiClient.SubmitReviewAsync(entity, cancellationToken);

            var result = new ReviewResult
            {
                ApplicationId = entity.Id,
                IsPassed = reviewResult.IsPassed,
                Summary = reviewResult.Summary,
                ReviewedAt = DateTime.UtcNow,
                AgentVersion = "1.0.0",
            };

            foreach (var finding in reviewResult.Findings)
            {
                result.Findings.Add(new ReviewFinding
                {
                    Category = finding.Category,
                    Severity = finding.Severity,
                    Description = finding.Description,
                    Suggestion = finding.Suggestion,
                });
            }

            entity.ReviewResults.Add(result);
            entity.Status = reviewResult.IsPassed ? "Approved" : "Rejected";
            entity.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            entity.Status = "Pending";
            entity.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw;
        }

        return MapDetail(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        Db.Set<ApprovalApplication>().Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ApprovalDetailDto> AddAttachmentAsync(
        Guid applicationId, string fileName, string fileType, string filePath, string attachmentType, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(applicationId, cancellationToken);

        var attachment = new ApprovalAttachment
        {
            ApplicationId = applicationId,
            FileName = fileName,
            FileType = fileType,
            FilePath = filePath,
            AttachmentType = attachmentType,
        };

        entity.Attachments.Add(attachment);
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapDetail(entity);
    }

    private async Task<ApprovalApplication> GetRequiredEntityAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await Db.Set<ApprovalApplication>()
            .Include(a => a.Attachments)
            .Include(a => a.ReviewResults)
                .ThenInclude(r => r.Findings)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("指定审批申请不存在");

        return entity;
    }

    private static ApprovalDetailDto MapDetail(ApprovalApplication entity)
    {
        return new ApprovalDetailDto(
            entity.Id,
            entity.ApplicantName,
            entity.ApplicantIdCard,
            entity.EnterpriseName,
            entity.EnterpriseLicenseNo,
            entity.WaterIntakeLocation,
            entity.WaterIntakePurpose,
            entity.WaterIntakeAmount,
            entity.ApplicationDate,
            entity.Status,
            entity.Attachments.Select(a => new ApprovalAttachmentDto(
                a.Id, a.FileName, a.FileType, a.AttachmentType, a.CreatedAt)).ToList(),
            entity.ReviewResults.Select(r => new ReviewResultDto(
                r.Id, r.IsPassed, r.Summary, r.ReviewedAt, r.AgentVersion,
                r.Findings.Select(f => new ReviewFindingDto(
                    f.Id, f.Category, f.Severity, f.Description, f.Suggestion)).ToList())).ToList(),
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
