using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Quesify.IdentityService.Core.Entities;

namespace Quesify.IdentityService.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "Identity");
        builder.Property(p => p.Id).HasColumnType("uniqueidentifier");
        builder.Property(p => p.Id).HasDefaultValueSql("newsequentialid()");
    }
}