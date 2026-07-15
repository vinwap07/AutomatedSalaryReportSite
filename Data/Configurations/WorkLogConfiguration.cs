using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

/// <summary>
/// Конфигурация таблицы отчетов о выполненной работе
/// </summary>
public class WorkLogConfiguration : IEntityTypeConfiguration<WorkLog>
{
    public void Configure(EntityTypeBuilder<WorkLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.WorkCost, costBuilder =>
        {
            costBuilder.Property(c => c.Rate)
                .HasColumnName("work_cost_rate")
                .HasPrecision(18, 2);

            costBuilder.Property(c => c.Volume)
                .HasColumnName("work_cost_volume")
                .HasPrecision(18, 2);

            costBuilder.Property(c => c.UnitOfMeasure)
                .HasColumnName("work_cost_unit_of_measure")
                .HasConversion<string>()
                .HasMaxLength(30);
        });
    }
}