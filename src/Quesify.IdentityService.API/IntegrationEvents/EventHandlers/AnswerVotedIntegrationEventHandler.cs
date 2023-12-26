using Microsoft.AspNetCore.Identity;
using Quesify.IdentityService.API.Controllers;
using Quesify.IdentityService.API.IntegrationEvents.Events;
using Quesify.IdentityService.Core.Entities;
using Quesify.SharedKernel.EventBus.Abstractions;

namespace Quesify.IdentityService.API.IntegrationEvents.EventHandlers;

public class AnswerVotedIntegrationEventHandler : IIntegrationEventHandler<AnswerVotedIntegrationEvent>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UsersController> _logger;

    public AnswerVotedIntegrationEventHandler(
        UserManager<User> userManager,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task HandleAsync(AnswerVotedIntegrationEvent integrationEvent)
    {
        _logger.Here().LogInformation("Handling integration event: {EventName} - ({@IntegrationEvent})", nameof(QuestionVotedIntegrationEventHandler), integrationEvent);

        var user = await _userManager.FindByIdAsync(integrationEvent.AnswerOwnerUserId.ToString());
        if (user == null)
        {
            _logger.Here().LogError("The user {UserId} was not found.", integrationEvent.AnswerOwnerUserId);
            return;
        }

        user.Score += integrationEvent.UserScoreAction;

        var updateUserResult = await _userManager.UpdateAsync(user);
        if (!updateUserResult.Succeeded)
        {
            _logger.Here().LogError("The user {UserId} was not updated: {@UpdateResult}", integrationEvent.AnswerOwnerUserId, updateUserResult);
        }
    }
}
