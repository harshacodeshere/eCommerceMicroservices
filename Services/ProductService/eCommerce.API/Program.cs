using eCommerce.Infrastructure;
using eCommerce.Core;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using eCommerce.Core.Validations;
using eCommerce.API.Middlewares;
using eCommerce.API.APIEndpoints;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCore();

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(ProductAddRequestValidator).Assembly);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionMiddleware();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapProductAPIEndpoints();

app.Run();
