using Microsoft.AspNetCore.Identity;
using Quesify.IdentityService.API.Controllers;
using Quesify.IdentityService.API.IntegrationEvents.Events;
using Quesify.IdentityService.Core.Entities;
using Quesify.SharedKernel.EventBus.Abstractions;
using Quesify.SharedKernel.Utilities.Exceptions;

namespace Quesify.IdentityService.API.IntegrationEvents.EventHandlers;

public class QuestionVotedIntegrationEventHandler : IIntegrationEventHandler<QuestionVotedIntegrationEvent>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UsersController> _logger;

    public QuestionVotedIntegrationEventHandler(
        UserManager<User> userManager,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task HandleAsync(QuestionVotedIntegrationEvent integrationEvent)
    {
        _logger.Here().LogInformation("Handling integration event: {EventName} - ({@IntegrationEvent})", nameof(QuestionVotedIntegrationEventHandler), integrationEvent);

        var user = await _userManager.FindByIdAsync(integrationEvent.QuestionOwnerUserId.ToString());
        if (user == null)
        {
            _logger.Here().LogError("The user {UserId} was not found.", integrationEvent.QuestionOwnerUserId);
            return;
        }

        user.Score += integrationEvent.UserScoreAction;

        var updateUserResult = await _userManager.UpdateAsync(user);
        if (!updateUserResult.Succeeded)
        {
            _logger.Here().LogError("The user {UserId} was not updated: {@UpdateResult}", integrationEvent.QuestionOwnerUserId ,updateUserResult);
        }
    }
}
