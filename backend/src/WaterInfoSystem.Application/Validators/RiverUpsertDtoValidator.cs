using FluentValidation;
using WaterInfoSystem.Application.Contracts.Rivers;

namespace WaterInfoSystem.Application.Validators;

public class RiverUpsertDtoValidator : AbstractValidator<RiverUpsertDto>
{
    public RiverUpsertDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("河道名称不能为空");
        RuleFor(x => x.Length).GreaterThan(0).WithMessage("河道长度必须大于0");
        RuleFor(x => x.Basin).NotEmpty().WithMessage("所属流域不能为空");
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}
