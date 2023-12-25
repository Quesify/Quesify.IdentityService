namespace Quesify.IdentityService.API.Models;

public class UserForRegisterRequest
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public UserForRegisterRequest()
    {
        UserName = null!;
        Email = null!;
        Password = null!;
    }
}
