using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Quesify.IdentityService.Core.Constants;
using Quesify.IdentityService.Core.Entities;

namespace Quesify.IdentityService.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "Identity");
        builder.Property(p => p.Id).HasDefaultValueSql("newsequentialid()").HasColumnType("uniqueidentifier");
        builder.Property(p => p.UserName).HasMaxLength(UserConstants.MaxUserNameLength).IsRequired();
        builder.Property(p => p.NormalizedUserName).HasMaxLength(UserConstants.MaxUserNameLength).IsRequired();
        builder.Property(p => p.Email).IsRequired();
        builder.Property(p => p.NormalizedEmail).IsRequired();
        builder.Property(p => p.Score).IsRequired();
        builder.Property(p => p.Location).HasMaxLength(UserConstants.MaxLocationLength);
        builder.Property(p => p.BirthDate).HasColumnType("datetime");
        builder.Property(p => p.WebSiteUrl).HasMaxLength(UserConstants.MaxWebSiteUrlLength);
        builder.Property(p => p.CreationDate).HasColumnType("datetime");
        builder.Property(p => p.DeletionDate).HasColumnType("datetime");
    }
}
