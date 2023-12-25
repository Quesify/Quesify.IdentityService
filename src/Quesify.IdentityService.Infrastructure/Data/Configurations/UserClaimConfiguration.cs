using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Quesify.IdentityService.Core.Entities;

namespace Quesify.IdentityService.Infrastructure.Data.Configurations;

public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable("UserClaims", "Identity");
    }
}
