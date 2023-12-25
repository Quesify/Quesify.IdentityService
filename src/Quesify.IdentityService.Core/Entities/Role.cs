using Microsoft.AspNetCore.Identity;

namespace Quesify.IdentityService.Core.Entities;

public class Role : IdentityRole<Guid>
{
    public Role()
        : base()
    {
    }

    public Role(string roleName)
        : base(roleName)
    {
    }
}
