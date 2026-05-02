using FluentValidation;
using WaterInfoSystem.Application.Contracts.Monitoring;

namespace WaterInfoSystem.Application.Validators;

public class MonitoringCreateDtoValidator : AbstractValidator<MonitoringCreateDto>
{
    public MonitoringCreateDtoValidator()
    {
        RuleFor(x => x.StationId).NotEmpty().WithMessage("站点ID不能为空");
        RuleFor(x => x.DataType).IsInEnum().WithMessage("监测数据类型无效");
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0).WithMessage("监测值不能为负");
        RuleFor(x => x.CollectedAt).NotEmpty().WithMessage("采集时间不能为空");
    }
}
