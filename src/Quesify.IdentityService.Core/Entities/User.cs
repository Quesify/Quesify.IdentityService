using Microsoft.AspNetCore.Identity;

namespace Quesify.IdentityService.Core.Entities;

public class User : IdentityUser<Guid>
{
    public int Score { get; set; }

    public string? About { get; set; }

    public string? Location { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? WebSiteUrl { get; set; }

    public string? ProfileImageUrl { get; set; }

    public DateTime CreationDate { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletionDate { get; set; }

    public User()
    {
    }

    public User(string userName)
        : this()
    {
        UserName = userName;
    }
}
