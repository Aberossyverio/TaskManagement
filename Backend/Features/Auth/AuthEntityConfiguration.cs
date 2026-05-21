using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskApi.Features.Auth.Domain;

namespace TaskApi.Features.Auth;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Fullname)
            .HasMaxLength(200);
    }
}

public class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(rt => rt.Jti)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(rt => rt.Token)
            .IsUnique();

        builder.HasIndex(rt => rt.Jti);

        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class RoleSeeder : IEntityTypeConfiguration<IdentityRole<int>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<int>> builder)
    {
        builder.HasData(
            new IdentityRole<int>
            {
                Id = 1,
                Name = "admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1"
            }
        );
    }
}

public class UserSeeder : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasData(new User
        {
            Id = 1,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@taskapi.com",
            NormalizedEmail = "ADMIN@TASKAPI.COM",
            EmailConfirmed = true,
            SecurityStamp = "ADMIN_SECURITY_STAMP",
            PasswordHash = "AQAAAAIAAYagAAAAEHKz8V7F3xXQKZwq0K9vYxJ3Z8xGxJ3Z8xGxJ3Z8xGxJ3Z8xGxJ3Z8xGxJ3Z8xGxJw==",
            Fullname = "Administrator"
        });
    }
}

public class UserRoleSeeder : IEntityTypeConfiguration<IdentityUserRole<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<int>> builder)
    {
        builder.HasData(
            new IdentityUserRole<int>
            {
                UserId = 1,
                RoleId = 1
            }
        );
    }
}