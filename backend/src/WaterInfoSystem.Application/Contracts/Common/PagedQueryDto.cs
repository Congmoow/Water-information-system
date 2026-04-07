using WaterInfoSystem.Domain.Constants;

namespace WaterInfoSystem.Application.Contracts.Common;

public record PagedQueryDto(
    int Page = SystemDefaults.DefaultPage,
    int PageSize = SystemDefaults.DefaultPageSize);
