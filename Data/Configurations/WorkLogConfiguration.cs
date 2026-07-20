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

        builder.HasIndex(x => x.Date);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.WorkType)
            .WithMany()
            .HasForeignKey(x => x.WorkTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReasonForAbsence)
            .WithMany()
            .HasForeignKey(x => x.ReasonForAbsenceId)
            .OnDelete(DeleteBehavior.Restrict);

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