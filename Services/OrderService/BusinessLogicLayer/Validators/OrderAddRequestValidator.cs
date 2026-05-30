using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validators;

public class OrderAddRequestValidator : AbstractValidator<OrderAddRequest>
{
    public OrderAddRequestValidator()
    {
        RuleFor(temp => temp.UserID)
            .NotEmpty().WithMessage("UserID is required.");
        RuleFor(temp => temp.OrderDate)
            .NotEmpty().WithMessage("OrderDate is required.");
        RuleFor(temp => temp.OrderItems)
            .NotEmpty().WithMessage("Order must contain at least one OrderItem.");
    }
}
