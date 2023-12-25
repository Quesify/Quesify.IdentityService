using Microsoft.AspNetCore.Identity;

namespace Quesify.IdentityService.API.Models;

public class UserForConfirmEmailResponse
{
    public bool Succeeded { get; set; }

    public IEnumerable<IdentityError>? Errors { get; set; }

    private UserForConfirmEmailResponse()
    {
    }

    public static UserForConfirmEmailResponse Succees()
    {
        return new UserForConfirmEmailResponse
        {
            Succeeded = true,
            Errors = null
        };
    }

    public static UserForConfirmEmailResponse Fail(IEnumerable<IdentityError> errors)
    {
        return new UserForConfirmEmailResponse
        {
            Succeeded = false,
            Errors = errors
        };
    }
}
