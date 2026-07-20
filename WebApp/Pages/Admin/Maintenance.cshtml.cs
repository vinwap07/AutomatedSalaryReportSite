using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Logging;
using WebApp.Middlewares;

namespace WebApp.Pages.Admin;

/// <summary>
/// Сервисная страница: выгрузка дампа базы данных и просмотр текстовых логов действий
/// </summary>
public class MaintenanceModel(
    AppDbContext dbContext,
    IAuditLogger auditLogger
    ) : PageModel
{
    private static readonly JsonSerializerOptions DumpJsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() },
    };

    [BindProperty(SupportsGet = true)]
    public string? LogFile { get; set; }

    public IReadOnlyList<string> LogFiles { get; private set; } = [];
    public string? LogContent { get; private set; }

    public async Task OnGetAsync()
    {
        LogFiles = auditLogger.GetLogFileNames();
        if (!string.IsNullOrEmpty(LogFile))
        {
            LogContent = await auditLogger.ReadLogFileAsync(LogFile);
        }
    }

    /// <summary>
    /// Выгружает все таблицы базы данных одним JSON-файлом
    /// </summary>
    public async Task<IActionResult> OnGetDumpAsync(CancellationToken cancellationToken)
    {
        var dump = new
        {
            CreatedAt = DateTimeOffset.Now,
            Users = await dbContext.Users.AsNoTracking()
                .Select(u => new { u.Id, u.Login, u.PasswordHash, u.Role })
                .ToListAsync(cancellationToken),
            Employees = await dbContext.Employees.AsNoTracking()
                .Select(e => new { e.Id, e.Name, e.Specialty, e.Comment, e.EquipmentId, e.UserId })
                .ToListAsync(cancellationToken),
            Equipment = await dbContext.Equipments.AsNoTracking()
                .Select(e => new { e.Id, e.Name, e.HasTracker })
                .ToListAsync(cancellationToken),
            WorkTypes = await dbContext.WorkTypes.AsNoTracking()
                .Select(w => new { w.Id, w.Name })
                .ToListAsync(cancellationToken),
            ReasonsForAbsence = await dbContext.ReasonForAbsences.AsNoTracking()
                .Select(r => new { r.Id, r.Name })
                .ToListAsync(cancellationToken),
            WorkLogs = await dbContext.WorkLogs.AsNoTracking()
                .Select(w => new
                {
                    w.Id,
                    w.EmployeeId,
                    w.Date,
                    w.WorkTypeId,
                    w.WorkHours,
                    Rate = w.WorkCost != null ? (decimal?)w.WorkCost.Rate : null,
                    Volume = w.WorkCost != null ? (decimal?)w.WorkCost.Volume : null,
                    UnitOfMeasure = w.WorkCost != null ? (Domain.ValueObjects.UnitOfMeasure?)w.WorkCost.UnitOfMeasure : null,
                    w.ReasonForAbsenceId,
                })
                .ToListAsync(cancellationToken),
        };

        var login = HttpContext.GetSessionData()?.Login ?? "?";
        await auditLogger.LogAsync(login, "Выгружен дамп базы данных",
            $"записей в журнале: {dump.WorkLogs.Count}, работников: {dump.Employees.Count}");

        var json = JsonSerializer.Serialize(dump, DumpJsonOptions);
        var fileName = $"dump-{DateTime.Now:yyyy-MM-dd-HHmm}.json";
        return File(Encoding.UTF8.GetBytes(json), "application/json", fileName);
    }

    /// <summary>
    /// Скачивает файл лога
    /// </summary>
    public async Task<IActionResult> OnGetDownloadLogAsync(string file)
    {
        var content = await auditLogger.ReadLogFileAsync(file);
        if (content == null)
        {
            TempData["Error"] = "Файл лога не найден";
            return RedirectToPage();
        }
        return File(Encoding.UTF8.GetBytes(content), "text/plain", file);
    }
}
