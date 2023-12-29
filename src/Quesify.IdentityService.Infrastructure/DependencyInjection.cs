using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quesify.IdentityService.Core.Entities;
using Quesify.IdentityService.Infrastructure.Data.Contexts;
using Quesify.SharedKernel.EventBus;
using Quesify.SharedKernel.Guids;
using Quesify.SharedKernel.Json;
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

        services.AddJsonSerializer(options => { options.UseNewtonsoft(services, settings => { }); });

        services.AddDbContext<IdentityContext>(options => { options.UseSqlServer(connectionString); });

        services.AddScoped<IdentityContextInitializer>();

        services.AddDefaultIdentity<User>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
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

        services.AddEventBus(options =>
        {
            options.ProjectName = "Quesify";
            options.ServiceName = "IdentityService";
            options.UseKafka(services, kafkaOptions =>
            {
                kafkaOptions.ProducerConfig.BootstrapServers = "localhost:9092"; //TODO: Read from appsettings.json
                kafkaOptions.ConsumerConfig.BootstrapServers = "localhost:9092";
                kafkaOptions.ConsumerConfig.GroupId = "Quesify.IdentityService.Group";
                kafkaOptions.ConsumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                kafkaOptions.ConsumerConfig.ClientId = "Quesify.IdentityService";
                kafkaOptions.ConsumerConfig.AllowAutoCreateTopics = true;
                kafkaOptions.AdminClientConfig.AllowAutoCreateTopics = true;
                kafkaOptions.AdminClientConfig.BootstrapServers = "localhost:9092";
            });
        });

        return services;
    }
}
