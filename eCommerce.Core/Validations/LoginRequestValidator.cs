using eCommerce.Core.DTO;
using FluentValidation;

namespace eCommerce.Core.Validations;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(loginRequest => loginRequest.Email)
            .NotEmpty().WithMessage("Email should not be empty")
            .EmailAddress().WithMessage("Email should be a valid email address");
        RuleFor(loginRequest => loginRequest.Password)
            .NotEmpty().WithMessage("Password should not be empty")
            .MinimumLength(6).WithMessage("Password should be at least 6 characters long");
    }
}
