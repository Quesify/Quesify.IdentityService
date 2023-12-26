using Microsoft.OpenApi.Models;
using Quesify.IdentityService.API.IntegrationEvents.EventHandlers;
using Quesify.SharedKernel.AspNetCore.Swagger.Filters;
using Quesify.SharedKernel.Utilities.Guards;
using Serilog;
using Serilog.Events;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(
        this IServiceCollection services,
        IConfiguration configuration,
        ConfigureHostBuilder hostBuilder)
    {
        Guard.Against.Null(services, nameof(services));
        Guard.Against.Null(configuration, nameof(configuration));
        Guard.Against.Null(hostBuilder, nameof(hostBuilder));

        services.AddControllers();

        services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo { Title = "Quesify.IdentityService.API", Version = "v1" });
            o.DocumentFilter<LowercaseDocumentFilter>();
        });

        services.AddEnableRequestBuffering();

        services.AddRequestTime();

        hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
            .WriteTo.Seq(context.Configuration.GetValue<string>("Seq:ServerUrl")!)
            .WriteTo.Console()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
            .AddCustomEnriches(services);
        });

        services.AddPushSerilogProperties();

        services.AddCustomAuthentication(
            configuration.GetValue<string>("AccessTokenOptions:Audience")!,
            configuration.GetValue<string>("AccessTokenOptions:Issuer")!,
            configuration.GetValue<string>("AccessTokenOptions:SecurityKey")!);

        services.AddHttpContextAccessor();
        services.AddHttpContextCurrentPrincipalAccessor();

        services.AddCustomExceptionHandler();

        services.AddTransient<QuestionVotedIntegrationEventHandler>();
        services.AddTransient<AnswerVotedIntegrationEventHandler>();

        return services;
    }
}
