using Application.Abstractions;
using Application.Abstractions.Tokens;
using Infrastructure.Carts;
using Infrastructure.Database;
using Infrastructure.Identity;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis.OM;

namespace Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' not found.");

        var redisConnectionString = configuration["Redis:ConnectionString"]
            ?? "redis://localhost:6379";
        services.AddSingleton(new RedisConnectionProvider(redisConnectionString));
        services.AddScoped<ICartStore, RedisCartStore>();
        services.AddHostedService<RedisIndexCreationService>();


        services.AddDbContext<ApplicationDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddOptions<JwtSettings>()
            .BindConfiguration(JwtSettings.SectionName)
            .ValidateOnStart();

        services.AddSingleton<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IIdentityService, IdentityService>();

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }
}
