using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DataAccessLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // Register your data access layer services here
        var connStringTemplate = configuration.GetConnectionString("MongoDB")!;
        var connString = connStringTemplate.Replace("${DB_HOST}", Environment.GetEnvironmentVariable("MONGO_HOST"))
                                           .Replace("${DB_PORT}", Environment.GetEnvironmentVariable("MONGO_PORT"));
        //var connString = connStringTemplate;

        services.AddSingleton<IMongoClient>(new MongoClient(connString));
        services.AddScoped<IMongoDatabase>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase("OrdersDatabase");
            //return client.GetDatabase(Environment.GetEnvironmentVariable("MONGO_DATABASE"));
        });

        services.AddScoped<IOrdersRepository, OrdersRepository>();

        return services;
    }
}
