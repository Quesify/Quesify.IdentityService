using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quesify.IdentityService.Core.Entities;
using Quesify.IdentityService.Infrastructure.Data.Contexts;
using Quesify.SharedKernel.Guids;
using Quesify.SharedKernel.TimeProviders;
using Quesify.SharedKernel.Utilities.Guards;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(services, nameof(services));
        Guard.Against.Null(connectionString, nameof(connectionString));

        services.AddGuid(options => { options.UseSequentialGuidGenerator(services); });

        services.AddTimeProvider(options => { options.UseUtcDateTime(services); });

        services.AddDbContext<IdentityContext>(options => { options.UseSqlServer(connectionString); });

        services.AddScoped<IdentityContextInitializer>();

        services.AddDefaultIdentity<User>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
        })
        .AddDefaultTokenProviders()
        .AddRoles<Role>()
        .AddEntityFrameworkStores<IdentityContext>();

        services.AddSecurity(options =>
        {
            options.Issuer = configuration.GetValue<string>("AccessTokenOptions:Issuer")!;
            options.Audience = configuration.GetValue<string>("AccessTokenOptions:Audience")!;
            options.Expiration = configuration.GetValue<int?>("AccessTokenOptions:Expiration") ?? 360;
            options.SecurityKey = configuration.GetValue<string>("AccessTokenOptions:SecurityKey")!;
        });

        return services;
    }
}
