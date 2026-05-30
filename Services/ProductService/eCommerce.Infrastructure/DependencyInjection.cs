using eCommerce.Core.RepositoryContracts;
using eCommerce.Infrastructure.ProductDbContext;
using eCommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Infrastructure;

public static class DependencyInjection 
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ProductDbConnectionString")));
        services.AddScoped<IProductsRepository, ProductRepository>();
        return services;
    }
}
