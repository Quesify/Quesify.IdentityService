using Quesify.IdentityService.API.IntegrationEvents.EventHandlers;
using Quesify.IdentityService.API.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration, builder.Host);

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var appStartedLogMessage = "{ApplicationName} started. Hosting environment: {Environment}";
logger.LogInformation(appStartedLogMessage);

var eventBus = app.Services.GetRequiredService<IEventBus>();
await eventBus.SubscribeAsync<QuestionVotedIntegrationEvent, QuestionVotedIntegrationEventHandler>();
await eventBus.SubscribeAsync<AnswerVotedIntegrationEvent, AnswerVotedIntegrationEventHandler>();

if (app.Environment.IsDevelopment())
{
    await app.InitializeDatabaseAsync();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseEnableRequestBuffering();

app.UseRequestTime();

app.UsePushSerilogPropertiesMiddleware();

//app.UseCustomHttpLoggingMiddleware();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler(o => { });

app.Run();
