using PartsPasal.Application.Interfaces;
using PartsPasal.Infrastructure.Data;
using PartsPasal.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using PartsPasal.Domain.Entities;

namespace PartsPasal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            // options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); // OR Npgsql for Postgres
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<User, Microsoft.AspNetCore.Identity.IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // DbContext is already registered above

        // Register generic repository
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

        // Register services here
        // services.AddScoped<IAIService, AIService>();
        // services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
