using Api.Endpoints.Auth;
using Api.Endpoints.Carts;
using Api.Endpoints.Products;
using Api.OpenApi;
using Api.Services;
using Application.Abstractions;
using Application.Behaviors;
using FluentValidation;
using Infrastructure;
using Mediator;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Ecommerce API";
        document.Info.Version = "v1";
        document.Info.Description = "Backend API for the ecommerce system built with DDD";
        return Task.CompletedTask;
    });
    options.AddDocumentTransformer<SecuritySchemeTransformer>();
    options.AddOperationTransformer<SecurityOperationTransformer>();
});

builder.Services.AddMediator(options =>
    options.ServiceLifetime = ServiceLifetime.Scoped);

builder.Services.AddScoped(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));

builder.Services.AddValidatorsFromAssembly(
    typeof(ValidationBehavior<,>).Assembly,
    includeInternalTypes: true);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Ecommerce API";
        options.Theme = ScalarTheme.Purple;
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = ["BearerAuth"]
        };
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapAuthEndpoints();
app.MapProductEndpoints();
app.MapCartEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
   .WithName("HealthCheck")
   .WithTags("Health");

app.Run();

public partial class Program;
