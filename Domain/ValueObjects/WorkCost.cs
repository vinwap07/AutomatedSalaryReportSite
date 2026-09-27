using System.ComponentModel.DataAnnotations;

namespace Domain.ValueObjects;

/// <summary>
/// Объем и стоимость выполненной работы 
/// </summary>
public record WorkCost
{
    /// <summary>
    /// Расценка за работу
    /// </summary>
    public decimal Rate { get; init; }
    
    /// <summary>
    /// Объем выполненной работы
    /// </summary>
    public decimal Volume { get; init; }
    
    /// <summary>
    /// Единица измерения объема выполненной работы (тонны, килограммы, количество рейсов, гектары, часы, штуки, дни, оклад)
    /// </summary>
    public UnitOfMeasure UnitOfMeasure { get; init; }
    
    /// <summary>
    /// Итоговая сумма за выполненную работу
    /// </summary>
    public decimal Total => Volume * Rate;

    /// <summary>
    /// Оплата окладом (фиксированная сумма)
    /// </summary>
    public bool IsSalary => UnitOfMeasure == UnitOfMeasure.Salary;

    private WorkCost() { }

    public WorkCost(decimal rate, decimal volume, UnitOfMeasure unitOfMeasure)
    {
        if (rate < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rate), "Rate cannot be negative");
        }

        if (volume < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(volume), "Volume cannot be negative");
        }
        
        Rate = rate;
        Volume = volume;
        UnitOfMeasure = unitOfMeasure;
    }

    /// <summary>
    /// Оплата окладом: фиксированная сумма без расценки и объема
    /// </summary>
    /// <param name="amount">Сумма оклада</param>
    public static WorkCost FromSalary(decimal amount) => new(amount, 1, UnitOfMeasure.Salary);
}

/// <summary>
/// Единица измерения объема работы
/// </summary>
public enum UnitOfMeasure
{
    [Display(Name = "Тонна")]
    Ton,
    
    [Display(Name = "Килограмм")]
    Kilogram,
    
    [Display(Name = "Рейс")]
    Trip,
    
    [Display(Name = "Гектар")]
    Hectare,
    
    [Display(Name = "Час")]
    Hour,
    
    [Display(Name = "Штука")]
    Piece,
    
    [Display(Name = "День")]
    Day,
    
    /// <summary>
    /// Фиксированная сумма (только для разнорабочих): расценка равна сумме, объем всегда 1
    /// </summary>
    [Display(Name = "Оклад")]
    Salary
}