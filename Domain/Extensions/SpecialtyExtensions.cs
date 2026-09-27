using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Extensions;

public static class SpecialtyExtensions
{
    /// <summary>
    /// Единицы измерения работ специалистов
    /// </summary>
    public static IReadOnlyList<UnitOfMeasure> SpecialistUnits { get; } =
    [
        UnitOfMeasure.Ton,
        UnitOfMeasure.Kilogram,
        UnitOfMeasure.Trip,
        UnitOfMeasure.Hectare,
        UnitOfMeasure.Hour,
    ];

    /// <summary>
    /// Единицы измерения работ разнорабочих: почасовая, поштучная, подневная оплата и оклад
    /// </summary>
    public static IReadOnlyList<UnitOfMeasure> LaborerUnits { get; } =
    [
        UnitOfMeasure.Hour,
        UnitOfMeasure.Piece,
        UnitOfMeasure.Day,
        UnitOfMeasure.Salary,
    ];

    /// <summary>
    /// Группирует виды работ по специальностям: сначала неназначенные, затем в порядке специальностей.
    /// Внутри группы виды работ отсортированы по названию
    /// </summary>
    public static List<IGrouping<Specialty?, WorkType>> GroupBySpecialty(this IEnumerable<WorkType> workTypes)
    {
        return workTypes
            .OrderBy(w => w.Name)
            .GroupBy(w => w.Specialty)
            .OrderBy(g => g.Key.HasValue)
            .ThenBy(g => g.Key)
            .ToList();
    }
}
