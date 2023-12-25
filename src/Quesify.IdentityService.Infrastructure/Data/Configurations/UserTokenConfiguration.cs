using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Quesify.IdentityService.Core.Entities;

namespace Quesify.IdentityService.Infrastructure.Data.Configurations;

public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("UserTokens", "Identity");
    }
}
