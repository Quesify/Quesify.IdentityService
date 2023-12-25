namespace Quesify.IdentityService.API.Models;

public class UserForLoginRequest
{
    public string Email { get; set; }

    public string Password { get; set; }

    public UserForLoginRequest()
    {
        Email = null!;
        Password = null!;
    }
}
