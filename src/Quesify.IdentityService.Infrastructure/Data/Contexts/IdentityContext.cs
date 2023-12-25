using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quesify.IdentityService.Core.Entities;
using System.Reflection;

namespace Quesify.IdentityService.Infrastructure.Data.Contexts;

public class IdentityContext :
    IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
