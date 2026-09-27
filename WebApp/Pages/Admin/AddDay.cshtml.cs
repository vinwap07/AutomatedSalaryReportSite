using Domain.Dtos;
using Domain.Entities;
using Domain.Extensions;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin;

/// <summary>
/// Внесение работ за день (как в экселе): таблица строк с выпадающими списками,
/// кнопки "повторить работников" и "повторить день" копируют данные предыдущего дня.
/// Строка разнорабочего вводится без техники и часов, со своими единицами измерения и окладом
/// </summary>
public class AddDayModel(
    IWorkLogService workLogService,
    IEmployeeService employeeService,
    IEquipmentService equipmentService,
    IWorkTypeService workTypeService,
    IReasonForAbsenceService reasonForAbsenceService,
    IAuditLogger auditLogger
    ) : PageModel
{
    private const int MaxPageSize = 100_000;

    [BindProperty(SupportsGet = true)]
    public DateOnly Date { get; set; }

    /// <summary>Строки ввода: работа или отсутствие одного работника</summary>
    [BindProperty]
    public List<WorkEntryInput> Rows { get; set; } = [];

    public List<EmployeeListItemDto> Employees { get; private set; } = [];
    public List<Domain.Entities.Equipment> EquipmentList { get; private set; } = [];
    public List<WorkType> WorkTypes { get; private set; } = [];

    /// <summary>Виды работ по специальностям (неназначенные первыми)</summary>
    public List<IGrouping<Specialty?, WorkType>> WorkTypeGroups { get; private set; } = [];

    public List<ReasonForAbsence> Reasons { get; private set; } = [];

    /// <summary>Последний день с записями до выбранной даты (источник для копирования)</summary>
    public DateOnly? PreviousDay { get; private set; }

    public List<string> RowErrors { get; } = [];

    public async Task OnGetAsync(string? prefill, CancellationToken cancellationToken)
    {
        if (Date == default)
        {
            Date = DateOnly.FromDateTime(DateTime.Today);
        }

        await LoadListsAsync(cancellationToken);
        PreviousDay = await FindPreviousDayAsync(cancellationToken);

        if (PreviousDay != null && prefill is "employees" or "full")
        {
            var previousLogs = (await workLogService.GetByFiltersAsync(new WorkLogFilters
            {
                From = PreviousDay,
                To = PreviousDay,
                PageSize = MaxPageSize,
            }, cancellationToken)).OrderBy(l => l.Employee.Name).ToList();

            Rows = prefill == "employees"
                ? previousLogs
                    .Select(l => l.Employee.Id)
                    .Distinct()
                    .Select(id => new WorkEntryInput { EmployeeId = id })
                    .ToList()
                : previousLogs
                    .Select(WorkEntryInput.FromLog)
                    .ToList();
        }

        if (Rows.Count == 0)
        {
            Rows = [new WorkEntryInput(), new WorkEntryInput(), new WorkEntryInput()];
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadListsAsync(cancellationToken);
        PreviousDay = await FindPreviousDayAsync(cancellationToken);

        var filledRows = Rows.Where(r => !r.IsEmpty).ToList();
        if (filledRows.Count == 0)
        {
            RowErrors.Add("Заполните хотя бы одну строку");
            return Page();
        }

        for (var i = 0; i < filledRows.Count; i++)
        {
            var row = filledRows[i];
            var rowNumber = i + 1;
            if (row.EmployeeId == null)
            {
                RowErrors.Add($"Строка {rowNumber}: выберите работника");
            }
            if (row.WorkTypeId == null && row.ReasonForAbsenceId == null)
            {
                RowErrors.Add($"Строка {rowNumber}: выберите вид работы или причину отсутствия");
            }
            // Списки загружены с учетом фермы: чужие записи в форму не попадут
            foreach (var error in ValidateReferences(row).Concat(row.ValidatePayment(IsLaborer(row.EmployeeId))))
            {
                RowErrors.Add($"Строка {rowNumber}: {error}");
            }
        }

        if (RowErrors.Count > 0)
        {
            Rows = filledRows;
            return Page();
        }

        var login = HttpContext.GetSessionData()?.Login ?? "?";
        var employeesById = Employees.ToDictionary(e => e.Id);

        foreach (var row in filledRows)
        {
            var isLaborer = IsLaborer(row.EmployeeId);
            var workCost = row.GetWorkCost(isLaborer);
            var workHours = row.GetWorkHours(isLaborer);

            await workLogService.CreateAsync(new CreateWorkLogRequest
            {
                EmployeeId = row.EmployeeId!.Value,
                Date = Date,
                EquipmentId = row.GetEquipmentId(isLaborer),
                WorkTypeId = row.WorkTypeId,
                WorkHours = workHours,
                WorkCost = workCost,
                ReasonForAbsenceId = row.ReasonForAbsenceId,
            }, cancellationToken);

            var employeeName = employeesById.TryGetValue(row.EmployeeId.Value, out var employee) ? employee.Name : "?";
            var workName = row.WorkTypeId != null
                ? WorkTypes.FirstOrDefault(w => w.Id == row.WorkTypeId)?.Name ?? "?"
                : Reasons.FirstOrDefault(r => r.Id == row.ReasonForAbsenceId)?.Name ?? "?";
            await auditLogger.LogAsync(login, "Добавлена запись о работе",
                $"{Date:yyyy-MM-dd} | {employeeName} | {workName} | часы: {workHours?.ToString() ?? "-"} | сумма: {(workCost?.Total.ToString("0.##") ?? "-")}");
        }

        TempData["Success"] = $"Добавлено записей: {filledRows.Count} (за {Date:dd.MM.yyyy})";
        return RedirectToPage("/Admin/Index", new
        {
            From = Date.ToString("yyyy-MM-dd"),
            To = Date.ToString("yyyy-MM-dd"),
        });
    }

    /// <summary>
    /// Является ли работник разнорабочим (от этого зависит набор полей строки)
    /// </summary>
    public bool IsLaborer(Guid? employeeId) =>
        Employees.FirstOrDefault(e => e.Id == employeeId)?.Specialty == Specialty.Laborer;

    /// <summary>
    /// Проверяет, что все выбранные в строке справочники относятся к ферме текущего пользователя
    /// </summary>
    private IEnumerable<string> ValidateReferences(WorkEntryInput row)
    {
        if (row.EmployeeId != null && Employees.All(e => e.Id != row.EmployeeId))
        {
            yield return "работник не найден";
        }
        if (row.EquipmentId != null && EquipmentList.All(e => e.Id != row.EquipmentId))
        {
            yield return "техника не найдена";
        }
        if (row.WorkTypeId != null && WorkTypes.All(w => w.Id != row.WorkTypeId))
        {
            yield return "вид работы не найден";
        }
        if (row.ReasonForAbsenceId != null && Reasons.All(r => r.Id != row.ReasonForAbsenceId))
        {
            yield return "причина отсутствия не найдена";
        }
    }

    private async Task LoadListsAsync(CancellationToken cancellationToken)
    {
        Employees = (await employeeService.GetAllAsync(cancellationToken))
            .OrderBy(e => e.Name).ToList();
        EquipmentList = (await equipmentService.GetAllAsync(cancellationToken))
            .OrderBy(e => e.Name).ToList();
        WorkTypes = (await workTypeService.GetAllAsync(cancellationToken))
            .OrderBy(w => w.Name).ToList();
        WorkTypeGroups = WorkTypes.GroupBySpecialty();
        Reasons = (await reasonForAbsenceService.GetAllAsync(cancellationToken))
            .OrderBy(r => r.Name).ToList();
    }

    private async Task<DateOnly?> FindPreviousDayAsync(CancellationToken cancellationToken)
    {
        var earlierLogs = await workLogService.GetByFiltersAsync(new WorkLogFilters
        {
            To = Date.AddDays(-1),
            PageSize = MaxPageSize,
        }, cancellationToken);
        var dates = earlierLogs.Select(l => l.Date).ToList();
        return dates.Count > 0 ? dates.Max() : null;
    }
}
