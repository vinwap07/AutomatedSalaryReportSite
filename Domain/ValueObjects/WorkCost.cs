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
    /// Единица измерения объема выполненной работы (тонны, количество рейсов, гектары)
    /// </summary>
    public UnitOfMeasure UnitOfMeasure { get; init; }
    
    /// <summary>
    /// Итоговая сумма за выполненную работу
    /// </summary>
    public decimal Total => Volume * Rate;
    
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
}

/// <summary>
/// Единица измерения объема работы
/// </summary>
public enum UnitOfMeasure
{
    [Display(Name = "Тонна")]
    Ton,
    
    [Display(Name = "Рейс")]
    Trip,
    
    [Display(Name = "Гектар")]
    Hectare
}