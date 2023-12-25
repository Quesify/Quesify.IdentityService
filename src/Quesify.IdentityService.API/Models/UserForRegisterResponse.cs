using Microsoft.AspNetCore.Identity;

namespace Quesify.IdentityService.API.Models;

public class UserForRegisterResponse
{
    public bool Succeeded { get; set; }

    public IEnumerable<IdentityError>? Errors { get; set; }

    private UserForRegisterResponse()
    {
    }

    public static UserForRegisterResponse Succees()
    {
        return new UserForRegisterResponse
        {
            Succeeded = true,
            Errors = null
        };
    }

    public static UserForRegisterResponse Fail(IEnumerable<IdentityError> errors)
    {
        return new UserForRegisterResponse
        {
            Succeeded = false,
            Errors = errors
        };
    }
}
