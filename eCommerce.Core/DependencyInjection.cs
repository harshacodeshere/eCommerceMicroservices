using eCommerce.Core.Validations;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using eCommerce.Core.Mappers;
using eCommerce.Core.ServiceContracts;
using eCommerce.Core.Services;

namespace eCommerce.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();
        services.AddScoped<IProductService, ProductService>();
        services.AddAutoMapper(typeof(ProductAddRequestMapping).Assembly);
        return services;
    }
}
