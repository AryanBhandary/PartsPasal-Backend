using PartsPasal.Application.Interfaces;
using PartsPasal.Infrastructure.Data;
using PartsPasal.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using PartsPasal.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PartsPasal.Infrastructure.Services;
using PartsPasal.Application.Services;


namespace PartsPasal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            // options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))); // OR Npgsql for Postgres
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<User, Microsoft.AspNetCore.Identity.IdentityRole<int>>()
            .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey is missing from configuration. Please set it in appsettings.json.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        // DbContext is already registered above

        // Register generic repository
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

        // Register services here
        // services.AddScoped<IAIService, AIService>();
        // services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IStaffManagementService, StaffManagementService>();

        // Register customer service
        services.AddScoped<ICustomerService, CustomerService>();
        //register part service
        services.AddScoped<IPartService, PartService>();
        //register vendor service 
        services.AddScoped<IVendorService, VendorService>();
        //register purchase service
        services.AddScoped<IPurchaseService, PurchaseService>();
        return services;
    }
}
