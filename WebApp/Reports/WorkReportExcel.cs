using ClosedXML.Excel;
using Domain.Extensions;
using WebApp.Pages.Reports;

namespace WebApp.Reports;

internal static class WorkReportExcel
{
    private const string DateFormat = "dd.mm.yyyy";
    private const string MoneyFormat = "#,##0.00";

    public static byte[] Create(IndexModel report)
    {
        using var workbook = new XLWorkbook();

        AddSummary(workbook, report);
        AddEntries(workbook, report);
        AddDailyTotals(workbook, report);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream, new SaveOptions { EvaluateFormulasBeforeSaving = true });
        return stream.ToArray();
    }

    private static void AddSummary(XLWorkbook workbook, IndexModel report)
    {
        var sheet = workbook.AddWorksheet("Итоги");
        Prepare(sheet, [32, 16, 16, 16, 17]);
        Heading(sheet, 1, 5, "Итоги за период");
        WriteRow(sheet, 2, "Период", report.From, report.To);
        sheet.Range("B2:C2").Style.DateFormat.Format = DateFormat;
        WriteRow(sheet, 3, "Работник", report.OwnEmployee?.Name
            ?? report.Employees.Find(employee => employee.Id == report.EmployeeId)?.Name
            ?? (report.ShowEmployeeColumn ? "Все работники" : "—"));

        Header(sheet, 5, "Показатель", "Значение");
        WriteRow(sheet, 6, "Итого заработано, ₽", report.TotalSum);
        sheet.Cell("B6").Style.NumberFormat.Format = MoneyFormat;
        WriteRow(sheet, 7, "Часов", report.TotalHours);
        WriteRow(sheet, 8, "Дней с работой", report.WorkDaysCount);
        WriteRow(sheet, 9, "Отсутствий", report.AbsenceCount);

        if (report.EmployeeSummary.Count == 0)
        {
            return;
        }

        Heading(sheet, 11, 5, "Сводка по работникам");
        Header(sheet, 12, "Работник", "Дней", "Часов", "Отсутствий", "Сумма, ₽");
        var row = 13;
        foreach (var employee in report.EmployeeSummary)
        {
            WriteRow(sheet, row++, employee.Name, employee.WorkDays, employee.Hours, employee.Absences, employee.Sum);
        }

        WriteRow(sheet, row, "Итого", report.WorkDaysCount, report.TotalHours, report.AbsenceCount, report.TotalSum);
        sheet.Range(row, 1, row, 5).Style.Font.Bold = true;
        sheet.Range(13, 5, row, 5).Style.NumberFormat.Format = MoneyFormat;
    }

    private static void AddEntries(XLWorkbook workbook, IndexModel report)
    {
        const int headerRow = 5;
        var sheet = workbook.AddWorksheet("Записи");
        Prepare(sheet, [15, 30, 34, 28, 24, 12, 17, 14, 17, 17]);
        Heading(sheet, 1, 10, "Работы и отсутствия");
        sheet.Range("A2:J2").Merge().Value = "Для выбора периода и категорий откройте фильтры в заголовках столбцов.";
        sheet.Range("A3:E3").Merge().Value = "Итого по видимым записям · часы и сумма";
        sheet.Range("A3:J3").Style.Font.Bold = true;
        sheet.Cell("J3").Style.NumberFormat.Format = MoneyFormat;
        Header(sheet, headerRow, "Дата", "Работник", "Вид работы", "Техника", "Причина отсутствия",
            "Часы", "Расценка, ₽", "Объём", "Ед. изм.", "Сумма, ₽");
        sheet.SheetView.FreezeRows(headerRow);

        var row = headerRow + 1;
        foreach (var day in report.Days)
        {
            foreach (var log in day.Logs)
            {
                var cost = log.WorkCost;
                var piecework = cost is { IsSalary: false } ? cost : null;
                WriteRow(sheet, row++, log.Date, log.Employee.Name, log.WorkType?.Name, log.Equipment?.Name,
                    log.ReasonForAbsence?.Name, log.WorkHours, piecework?.Rate, piecework?.Volume,
                    cost?.UnitOfMeasure.GetDisplayName(), cost?.Total);
            }
        }

        var lastRow = row - 1;
        sheet.Range(headerRow, 1, lastRow, 10).SetAutoFilter();
        if (lastRow == headerRow)
        {
            WriteRow(sheet, row, "Записей нет");
            sheet.Cell("F3").Value = 0;
            sheet.Cell("J3").Value = 0;
            return;
        }

        sheet.Range(headerRow + 1, 1, lastRow, 1).Style.DateFormat.Format = DateFormat;
        sheet.Range(headerRow + 1, 7, lastRow, 7).Style.NumberFormat.Format = MoneyFormat;
        sheet.Range(headerRow + 1, 8, lastRow, 8).Style.NumberFormat.Format = "0.##";
        sheet.Range(headerRow + 1, 10, lastRow, 10).Style.NumberFormat.Format = MoneyFormat;
        sheet.Cell("F3").FormulaA1 = $"SUBTOTAL(109,F{headerRow + 1}:F{lastRow})";
        sheet.Cell("J3").FormulaA1 = $"SUBTOTAL(109,J{headerRow + 1}:J{lastRow})";
    }

    private static void AddDailyTotals(XLWorkbook workbook, IndexModel report)
    {
        var sheet = workbook.AddWorksheet("По дням");
        Prepare(sheet, [16, 14, 17]);
        Heading(sheet, 1, 3, "Итоги по дням");
        Header(sheet, 3, "Дата", "Часы", "Сумма, ₽");
        sheet.SheetView.FreezeRows(3);
        var row = 4;
        foreach (var day in report.Days)
        {
            WriteRow(sheet, row++, day.Date, day.DayHours, day.DaySum);
        }

        if (row > 4)
        {
            sheet.Range(4, 1, row - 1, 1).Style.DateFormat.Format = DateFormat;
            sheet.Range(4, 3, row - 1, 3).Style.NumberFormat.Format = MoneyFormat;
        }
    }

    private static void Prepare(IXLWorksheet sheet, double[] widths)
    {
        sheet.ShowGridLines = false;
        for (var column = 0; column < widths.Length; column++)
        {
            sheet.Column(column + 1).Width = widths[column];
        }
    }

    private static void Heading(IXLWorksheet sheet, int row, int columns, string title)
    {
        var heading = sheet.Range(row, 1, row, columns).Merge();
        heading.Value = title;
        heading.Style.Font.Bold = true;
        heading.Style.Font.FontSize = 14;
        sheet.Row(row).Height = 23;
    }

    private static void Header(IXLWorksheet sheet, int row, params object?[] labels)
    {
        WriteRow(sheet, row, labels);
        var header = sheet.Range(row, 1, row, labels.Length);
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F513F");
        header.Style.Font.FontColor = XLColor.White;
        header.Style.Font.Bold = true;
        sheet.Row(row).Height = 20;
    }

    private static void WriteRow(IXLWorksheet sheet, int row, params object?[] values)
    {
        for (var column = 0; column < values.Length; column++)
        {
            sheet.Cell(row, column + 1).Value = values[column] switch
            {
                string text => text,
                DateOnly date => date.ToDateTime(TimeOnly.MinValue),
                int number => number,
                decimal number => number,
                _ => Blank.Value,
            };
        }
    }
}
