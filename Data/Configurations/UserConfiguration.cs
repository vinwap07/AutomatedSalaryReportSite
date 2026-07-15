using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Login)
            .HasMaxLength(100);
        
        builder.Property(x => x.PasswordHash)
            .HasMaxLength(255);
        
        builder.Property(x => x.Role)
            .HasConversion<string>()
            .HasMaxLength(30);
        
        builder.HasIndex(x => x.Login);
    }
}