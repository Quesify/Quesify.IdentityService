using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Quesify.IdentityService.Core.Entities;

namespace Quesify.IdentityService.Infrastructure.Data.Configurations;

public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable("UserLogins", "Identity");
    }
}
