using Microsoft.AspNetCore.Identity;
using Quesify.SharedKernel.Security.Tokens;

namespace Quesify.IdentityService.API.Models;

public class UserForLoginResponse
{
    public bool Succeeded { get; set; }

    public bool Failed { get; set; }

    public bool IsLockedOut { get; set; }

    public bool RequiresTwoFactor { get; set; }

    public bool RequiresEmailConfirmation { get; set; }

    public AccessToken? AccessToken { get; set; }

    private UserForLoginResponse()
    {
    }

    public static UserForLoginResponse Success(AccessToken accessToken)
    {
        return new UserForLoginResponse() { Succeeded = true, AccessToken = accessToken };
    }

    public static UserForLoginResponse Fail()
    {
        return new UserForLoginResponse() { Failed = true };
    }

    public static UserForLoginResponse LockedOut()
    {
        return new UserForLoginResponse() { IsLockedOut = true };
    }

    public static UserForLoginResponse TwoFactorRequired()
    {
        return new UserForLoginResponse() { RequiresTwoFactor = true };
    }

    public static UserForLoginResponse EmailConfirmationRequired()
    {
        return new UserForLoginResponse() { RequiresEmailConfirmation = true };
    }

    public static explicit operator UserForLoginResponse(SignInResult signInResult)
    {
        if (signInResult.Succeeded)
        {
            return new UserForLoginResponse() { Succeeded = true };
        }
        else if (signInResult.IsNotAllowed)
        {
            return EmailConfirmationRequired();
        }
        else if (signInResult.IsLockedOut)
        {
            return LockedOut();
        }
        else if (signInResult.RequiresTwoFactor)
        {
            return TwoFactorRequired();
        }
        else
        {
            return Fail();
        }
    }
}

