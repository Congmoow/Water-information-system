using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Application.Services;

public class AiServiceClient : IAiServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    public AiServiceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["AiService:BaseUrl"] ?? "http://localhost:8000";
    }

    public async Task<AiReviewResult> SubmitReviewAsync(ApprovalApplication application, CancellationToken cancellationToken)
    {
        var request = new
        {
            application_id = application.Id.ToString(),
            applicant_name = application.ApplicantName,
            applicant_id_card = application.ApplicantIdCard,
            enterprise_name = application.EnterpriseName,
            enterprise_license_no = application.EnterpriseLicenseNo,
            water_intake_location = application.WaterIntakeLocation,
            water_intake_purpose = application.WaterIntakePurpose,
            water_intake_amount = application.WaterIntakeAmount,
            materials = application.Attachments.Select(a => new
            {
                name = a.FileName,
                file_type = a.FileType,
                file_path = a.FilePath,
                attachment_type = a.AttachmentType,
            }).ToList(),
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}/review/submit", request, JsonOptions, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AiReviewResult>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("AI 服务返回了空结果");
    }
}
