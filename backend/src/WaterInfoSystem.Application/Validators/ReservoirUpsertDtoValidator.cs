using FluentValidation;
using WaterInfoSystem.Application.Contracts.Reservoirs;

namespace WaterInfoSystem.Application.Validators;

public class ReservoirUpsertDtoValidator : AbstractValidator<ReservoirUpsertDto>
{
    public ReservoirUpsertDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("水库名称不能为空");
        RuleFor(x => x.Location).NotEmpty().WithMessage("所在位置不能为空");
        RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("容量必须大于0");
        RuleFor(x => x.ManagementUnit).NotEmpty().WithMessage("管理单位不能为空");
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).WithMessage("纬度范围在 -90 到 90 之间");
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).WithMessage("经度范围在 -180 到 180 之间");
    }
}
