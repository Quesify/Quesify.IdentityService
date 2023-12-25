namespace Quesify.IdentityService.API.Models;

public class UserForConfirmEmailRequest
{
    public Guid UserId { get; set; }

    public string Code { get; set; }

    public UserForConfirmEmailRequest()
    {
        Code = null!;
    }
}
