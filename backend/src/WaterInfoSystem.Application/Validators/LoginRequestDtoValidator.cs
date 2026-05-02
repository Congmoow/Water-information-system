using FluentValidation;
using WaterInfoSystem.Application.Contracts.Auth;

namespace WaterInfoSystem.Application.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("用户名不能为空");
        RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
    }
}
