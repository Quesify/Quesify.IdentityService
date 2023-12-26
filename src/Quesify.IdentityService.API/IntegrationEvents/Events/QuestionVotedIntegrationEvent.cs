using Quesify.EventBus.Events;

namespace Quesify.IdentityService.API.IntegrationEvents.Events;

public class QuestionVotedIntegrationEvent : IntegrationEvent
{
    public Guid QuestionOwnerUserId { get; set; }

    public int UserScoreAction { get; set; }

    public QuestionVotedIntegrationEvent(
        Guid questionOwnerUserId,
        int userScoreAction,
        Guid id,
        DateTime creationDate)
        : base(id, creationDate)
    {
        QuestionOwnerUserId = questionOwnerUserId;
        UserScoreAction = userScoreAction;
    }
}
