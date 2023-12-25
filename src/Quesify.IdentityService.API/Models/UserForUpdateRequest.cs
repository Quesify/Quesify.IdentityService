namespace Quesify.IdentityService.API.Models;

public class UserForUpdateRequest
{
    public string? About { get; set; }

    public string? Location { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? WebSiteUrl { get; set; }

    public string? ProfileImageUrl { get; set; }
}
