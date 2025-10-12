using eCommerce.Core.DTO;
using FluentValidation;

namespace eCommerce.Core.Validations;

public class ProductUpdateValidator : AbstractValidator<ProductUpdateRequest>
{
    public ProductUpdateValidator()
    {
        RuleFor(ProductDto => ProductDto.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");
        RuleFor(ProductDto => ProductDto.ProductName)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");
        RuleFor(ProductDto => ProductDto.Category)
            .IsInEnum().WithMessage("Category is required.");
        RuleFor(ProductDto => ProductDto.UnitPrice)
            .InclusiveBetween(0, double.MaxValue).WithMessage("Unit price must be greater than zero.");
        RuleFor(ProductDto => ProductDto.QuantityInStock)
            .InclusiveBetween(0, int.MaxValue).WithMessage($"Quantity in stock must be between zero and {int.MaxValue}");

    }
}
