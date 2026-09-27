using Domain.Dtos;
using Domain.ValueObjects;

namespace WebApp.Pages.Admin;

/// <summary>
/// Запись о работе (или отсутствии) из формы ввода.
/// Набор полей зависит от специальности работника: у разнорабочего нет техники и отдельных часов,
/// свои единицы измерения (часы, штуки, дни) и оплата окладом — фиксированной суммой
/// </summary>
public class WorkEntryInput
{
    public Guid? EmployeeId { get; set; }
    public Guid? EquipmentId { get; set; }
    public Guid? WorkTypeId { get; set; }
    public int? WorkHours { get; set; }
    public decimal? Rate { get; set; }
    public decimal? Volume { get; set; }

    /// <summary>Единица измерения работы специалиста</summary>
    public UnitOfMeasure? Unit { get; set; }

    /// <summary>Единица измерения работы разнорабочего (свой список, включая оклад)</summary>
    public UnitOfMeasure? LaborerUnit { get; set; }

    /// <summary>Сумма оклада (для разнорабочего с оплатой окладом)</summary>
    public decimal? Amount { get; set; }

    public Guid? ReasonForAbsenceId { get; set; }

    public bool IsEmpty =>
        EmployeeId == null && EquipmentId == null && WorkTypeId == null && WorkHours == null &&
        Rate == null && Volume == null && Amount == null && ReasonForAbsenceId == null;

    public static WorkEntryInput FromLog(WorkLogListItemDto log) =>
        Create(log.Employee.Id, log.Equipment?.Id, log.WorkType?.Id, log.WorkHours, log.WorkCost, log.ReasonForAbsence?.Id);

    public static WorkEntryInput FromLog(WorkLogDetailsDto log) =>
        Create(log.Employee.Id, log.Equipment?.Id, log.WorkType?.Id, log.WorkHours, log.WorkCost, log.ReasonForAbsence?.Id);

    /// <summary>
    /// Оплата окладом (учитывается только у разнорабочих)
    /// </summary>
    public bool IsSalary(bool isLaborer) => isLaborer && LaborerUnit == UnitOfMeasure.Salary;

    /// <summary>
    /// Проверяет поля оплаты с учетом специальности работника
    /// </summary>
    public IEnumerable<string> ValidatePayment(bool isLaborer)
    {
        if (IsSalary(isLaborer))
        {
            if (Amount < 0)
            {
                yield return "сумма оклада не может быть отрицательной";
            }
            yield break;
        }

        if (!isLaborer && Unit == UnitOfMeasure.Salary)
        {
            yield return "оклад доступен только разнорабочим";
        }
        if (Rate != null ^ Volume != null)
        {
            yield return "для расчёта суммы нужны и расценка, и объём";
        }
        if (Rate < 0 || Volume < 0)
        {
            yield return "расценка и объём не могут быть отрицательными";
        }
    }

    /// <summary>
    /// Техника (у разнорабочих не указывается)
    /// </summary>
    public Guid? GetEquipmentId(bool isLaborer) => isLaborer ? null : EquipmentId;

    /// <summary>
    /// Отработанные часы. У разнорабочего они не вводятся отдельно: при почасовой оплате это объём работы
    /// </summary>
    public int? GetWorkHours(bool isLaborer)
    {
        if (!isLaborer)
        {
            return WorkHours;
        }

        return LaborerUnit == UnitOfMeasure.Hour && Volume != null
            ? (int)Math.Min(Math.Round(Volume.Value, MidpointRounding.AwayFromZero), int.MaxValue)
            : null;
    }

    /// <summary>
    /// Стоимость работы: оклад или расценка × объём (null, если оплата не указана)
    /// </summary>
    public WorkCost? GetWorkCost(bool isLaborer)
    {
        if (IsSalary(isLaborer))
        {
            return Amount != null ? WorkCost.FromSalary(Amount.Value) : null;
        }

        if (Rate == null || Volume == null)
        {
            return null;
        }

        var unit = isLaborer
            ? LaborerUnit ?? UnitOfMeasure.Hour
            : Unit ?? UnitOfMeasure.Ton;
        return new WorkCost(Rate.Value, Volume.Value, unit);
    }

    private static WorkEntryInput Create(
        Guid employeeId,
        Guid? equipmentId,
        Guid? workTypeId,
        int? workHours,
        WorkCost? workCost,
        Guid? reasonForAbsenceId)
    {
        var isSalary = workCost?.IsSalary == true;
        return new WorkEntryInput
        {
            EmployeeId = employeeId,
            EquipmentId = equipmentId,
            WorkTypeId = workTypeId,
            WorkHours = workHours,
            Rate = isSalary ? null : workCost?.Rate,
            Volume = isSalary ? null : workCost?.Volume,
            // Какой из списков единиц покажется, решает специальность работника в форме
            Unit = workCost?.UnitOfMeasure,
            LaborerUnit = workCost?.UnitOfMeasure,
            Amount = isSalary ? workCost!.Total : null,
            ReasonForAbsenceId = reasonForAbsenceId,
        };
    }
}
