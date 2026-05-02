using FluentValidation;
using WaterInfoSystem.Application.Contracts.Alarms;

namespace WaterInfoSystem.Application.Validators;

public class AlarmCreateDtoValidator : AbstractValidator<AlarmCreateDto>
{
    public AlarmCreateDtoValidator()
    {
        RuleFor(x => x.StationId).NotEmpty().WithMessage("站点ID不能为空");
        RuleFor(x => x.Message).NotEmpty().WithMessage("告警消息不能为空");
        RuleFor(x => x.AlarmType).IsInEnum();
        RuleFor(x => x.Level).IsInEnum();
    }
}
