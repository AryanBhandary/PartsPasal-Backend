using PartsPasal.Application.Interfaces;
using PartsPasal.Application.Services;
using PartsPasal.Infrastructure.Data;
using PartsPasal.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PartsPasal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            // options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); // OR Npgsql for Postgres
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Register repositories here
        // services.AddScoped<IVehiclePartRepository, VehiclePartRepository>();

        // Register services here
        // services.AddScoped<IAIService, AIService>();
        // services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
