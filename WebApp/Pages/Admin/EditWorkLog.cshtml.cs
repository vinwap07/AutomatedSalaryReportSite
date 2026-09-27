using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Extensions;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin;

/// <summary>
/// Редактирование одной записи о работе
/// </summary>
public class EditWorkLogModel(
    IWorkLogService workLogService,
    IEmployeeService employeeService,
    IEquipmentService equipmentService,
    IWorkTypeService workTypeService,
    IReasonForAbsenceService reasonForAbsenceService,
    IAuditLogger auditLogger
    ) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public DateOnly Date { get; set; }

    [BindProperty]
    public WorkEntryInput Entry { get; set; } = new();

    public List<EmployeeListItemDto> Employees { get; private set; } = [];
    public List<Domain.Entities.Equipment> EquipmentList { get; private set; } = [];
    public List<WorkType> WorkTypes { get; private set; } = [];

    /// <summary>Виды работ по специальностям (неназначенные первыми)</summary>
    public List<IGrouping<Specialty?, WorkType>> WorkTypeGroups { get; private set; } = [];

    public List<ReasonForAbsence> Reasons { get; private set; } = [];
    public List<string> Errors { get; } = [];

    /// <summary>Выбран разнорабочий: форма без техники и часов, со своими единицами измерения и окладом</summary>
    public bool IsLaborer =>
        Employees.FirstOrDefault(e => e.Id == Entry.EmployeeId)?.Specialty == Specialty.Laborer;

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadListsAsync(cancellationToken);

        try
        {
            var log = await workLogService.GetByIdAsync(Id, cancellationToken);
            Date = log.Date;
            Entry = WorkEntryInput.FromLog(log);
            return Page();
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Запись не найдена";
            return RedirectToPage("/Admin/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadListsAsync(cancellationToken);

        if (Entry.EmployeeId == null)
        {
            Errors.Add("Выберите работника");
        }
        if (Entry.WorkTypeId == null && Entry.ReasonForAbsenceId == null)
        {
            Errors.Add("Выберите вид работы или причину отсутствия");
        }
        Errors.AddRange(Entry.ValidatePayment(IsLaborer).Select(e => char.ToUpper(e[0]) + e[1..]));

        // Списки загружены с учетом фермы: чужие записи в форму не попадут
        if (Entry.EmployeeId != null && Employees.All(e => e.Id != Entry.EmployeeId))
        {
            Errors.Add("Работник не найден");
        }
        if (Entry.EquipmentId != null && EquipmentList.All(e => e.Id != Entry.EquipmentId))
        {
            Errors.Add("Техника не найдена");
        }
        if (Entry.WorkTypeId != null && WorkTypes.All(w => w.Id != Entry.WorkTypeId))
        {
            Errors.Add("Вид работы не найден");
        }
        if (Entry.ReasonForAbsenceId != null && Reasons.All(r => r.Id != Entry.ReasonForAbsenceId))
        {
            Errors.Add("Причина отсутствия не найдена");
        }

        if (Errors.Count > 0)
        {
            return Page();
        }

        try
        {
            var updated = await workLogService.UpdateAsync(new UpdateWorkLogRequest
            {
                Id = Id,
                EmployeeId = Entry.EmployeeId,
                Date = Date,
                EquipmentId = Entry.GetEquipmentId(IsLaborer),
                WorkTypeId = Entry.WorkTypeId,
                WorkHours = Entry.GetWorkHours(IsLaborer),
                WorkCost = Entry.GetWorkCost(IsLaborer),
                ReasonForAbsenceId = Entry.ReasonForAbsenceId,
            }, cancellationToken);

            var login = HttpContext.GetSessionData()?.Login ?? "?";
            await auditLogger.LogAsync(login, "Изменена запись о работе",
                $"{updated.Date:yyyy-MM-dd} | {updated.Employee.Name} | {updated.WorkType?.Name ?? updated.ReasonForAbsence?.Name ?? "-"}");

            TempData["Success"] = "Запись сохранена";
            return RedirectToPage("/Admin/Index", new
            {
                From = Date.ToString("yyyy-MM-dd"),
                To = Date.ToString("yyyy-MM-dd"),
            });
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Запись не найдена";
            return RedirectToPage("/Admin/Index");
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var log = await workLogService.GetByIdAsync(Id, cancellationToken);
            await workLogService.DeleteAsync(Id, cancellationToken);

            var login = HttpContext.GetSessionData()?.Login ?? "?";
            await auditLogger.LogAsync(login, "Удалена запись о работе",
                $"{log.Date:yyyy-MM-dd} | {log.Employee.Name} | {log.WorkType?.Name ?? log.ReasonForAbsence?.Name ?? "-"}");
            TempData["Success"] = "Запись удалена";
        }
        catch (NotFoundException)
        {
            TempData["Error"] = "Запись не найдена";
        }

        return RedirectToPage("/Admin/Index");
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
}
