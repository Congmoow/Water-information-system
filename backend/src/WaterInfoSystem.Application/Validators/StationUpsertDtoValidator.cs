using FluentValidation;
using WaterInfoSystem.Application.Contracts.Stations;

namespace WaterInfoSystem.Application.Validators;

public class StationUpsertDtoValidator : AbstractValidator<StationUpsertDto>
{
    public StationUpsertDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("站点名称不能为空");
        RuleFor(x => x.Type).IsInEnum().WithMessage("站点类型无效");
        RuleFor(x => x.Status).IsInEnum().WithMessage("站点状态无效");
        RuleFor(x => x.WarningThreshold).GreaterThanOrEqualTo(0).WithMessage("警戒阈值不能为负");
        RuleFor(x => x.CriticalThreshold).GreaterThanOrEqualTo(0).WithMessage("严重阈值不能为负");
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}
