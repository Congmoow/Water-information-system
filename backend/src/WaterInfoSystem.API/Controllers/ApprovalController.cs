using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterInfoSystem.Application.Contracts.Approvals;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/approval")]
public class ApprovalController : ControllerBase
{
    private readonly IApprovalService _approvalService;

    public ApprovalController(IApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ApprovalListItemDto>>>> Search(
        [FromQuery] ApprovalQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _approvalService.SearchAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<ApprovalListItemDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ApprovalDetailDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _approvalService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<ApprovalDetailDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ApprovalDetailDto>>> Create(
        [FromBody] ApprovalCreateDto request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _approvalService.CreateAsync(request, userId, cancellationToken);
        return Ok(ApiResponse<ApprovalDetailDto>.Success(result));
    }

    [HttpPost("{id:guid}/submit-review")]
    public async Task<ActionResult<ApiResponse<ApprovalDetailDto>>> SubmitForReview(
        Guid id, CancellationToken cancellationToken)
    {
        var result = await _approvalService.SubmitForReviewAsync(id, cancellationToken);
        return Ok(ApiResponse<ApprovalDetailDto>.Success(result));
    }

    [HttpPost("{id:guid}/attachments")]
    public async Task<ActionResult<ApiResponse<ApprovalDetailDto>>> AddAttachment(
        Guid id,
        [FromForm] string fileName,
        [FromForm] string fileType,
        [FromForm] string attachmentType,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var uploadsDir = Path.Combine("uploads", id.ToString());
        Directory.CreateDirectory(uploadsDir);

        var filePath = Path.Combine(uploadsDir, $"{Guid.NewGuid()}_{fileName}");
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        var result = await _approvalService.AddAttachmentAsync(
            id, fileName, fileType, filePath, attachmentType, cancellationToken);

        return Ok(ApiResponse<ApprovalDetailDto>.Success(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _approvalService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object?>.Success(null, "删除成功"));
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;
        return claim is not null ? Guid.Parse(claim) : Guid.Empty;
    }
}
