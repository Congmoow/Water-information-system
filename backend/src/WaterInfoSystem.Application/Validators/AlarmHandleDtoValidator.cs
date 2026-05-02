using FluentValidation;
using WaterInfoSystem.Application.Contracts.Alarms;

namespace WaterInfoSystem.Application.Validators;

public class AlarmHandleDtoValidator : AbstractValidator<AlarmHandleDto>
{
    public AlarmHandleDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum().WithMessage("告警状态无效");
    }
}
