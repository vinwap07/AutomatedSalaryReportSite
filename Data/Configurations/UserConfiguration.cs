using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

/// <summary>
/// Конфигурация таблицы пользователей
/// </summary>
public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Login)
            .HasMaxLength(100);

        builder.HasIndex(x => x.Login)
            .IsUnique();
        
        builder.Property(x => x.PasswordHash)
            .HasMaxLength(255);
        
        builder.Property(x => x.Role)
            .HasDefaultValue(Role.User)
            .HasConversion<string>()
            .HasMaxLength(30);
    }
}