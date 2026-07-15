using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

/// <summary>
/// Конфигурация таблицы сотрудников
/// </summary>
public class EmployeeConfiguration: IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasMaxLength(100);
        builder.Property(x => x.LastName)
            .HasMaxLength(100);
        builder.Property(x => x.Surname)
            .HasMaxLength(100);

        builder.Property(x => x.Specialty)
            .HasConversion<string>()
            .HasMaxLength(30);
    }
}