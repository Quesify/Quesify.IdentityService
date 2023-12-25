using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quesify.IdentityService.Core.Entities;

namespace Quesify.IdentityService.Infrastructure.Data.Configurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("RoleClaims", "Identity");
    }
}
