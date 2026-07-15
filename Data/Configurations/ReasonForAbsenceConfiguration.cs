using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

/// <summary>
/// Конфигурация таблицы причин пропуска работы
/// </summary>
public class ReasonForAbsenceConfiguration : IEntityTypeConfiguration<ReasonForAbsence>
{
    public void Configure(EntityTypeBuilder<ReasonForAbsence> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .HasMaxLength(255);
    }
}