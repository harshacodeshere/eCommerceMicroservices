using BusinessLogicLayer;
using BusinessLogicLayer.HttpClients;
using BusinessLogicLayer.Policies;
using DataAccessLayer;
using eCommerce.API.Middlewares;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddTransient<IUsersMicroservicePolicies, UsersMicroservicePolicies>();
builder.Services.AddTransient<IProductsMicroservicePolicies, ProductsMicroservicePolicies>();

builder.Services.AddHttpClient<UsersMicroserviceClient>(client =>
{
    //client.BaseAddress = new Uri($"http://{builder.Configuration["UsersMicroserviceName"]}:" +
    //                                    $"{builder.Configuration["UsersMicroservicePort"]}");
    client.BaseAddress = new Uri($"http://apigateway:8080");
}).AddPolicyHandler(
    builder.Services.BuildServiceProvider()
        .GetRequiredService<IUsersMicroservicePolicies>()
        .GetCombinedPolicy()
    );

builder.Services.AddHttpClient<ProductMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri("http://apigateway:8080");
}).AddPolicyHandler(
    builder.Services.BuildServiceProvider().GetRequiredService<IProductsMicroservicePolicies>().GetCombinedPolicy());

var app = builder.Build();

app.UseExceptionMiddleware();
app.UseRouting();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
