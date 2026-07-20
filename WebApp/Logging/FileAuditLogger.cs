using System.Text;

namespace WebApp.Logging;

/// <summary>
/// Журнал действий пользователей с записью в текстовые файлы logs/audit-*.log
/// </summary>
public class FileAuditLogger : IAuditLogger
{
    private readonly string _logDirectory;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public FileAuditLogger(IWebHostEnvironment environment)
    {
        _logDirectory = Path.Combine(environment.ContentRootPath, "logs");
        Directory.CreateDirectory(_logDirectory);
    }

    /// <inheritdoc />
    public async Task LogAsync(string login, string action, string details)
    {
        var now = DateTimeOffset.Now;
        var fileName = $"audit-{now:yyyy-MM}.log";
        var line = $"{now:yyyy-MM-dd HH:mm:ss} | {login} | {action} | {details}{Environment.NewLine}";

        await _writeLock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(Path.Combine(_logDirectory, fileName), line, Encoding.UTF8);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<string> GetLogFileNames()
    {
        return Directory.EnumerateFiles(_logDirectory, "audit-*.log")
            .Select(Path.GetFileName)
            .Where(name => name != null)
            .Select(name => name!)
            .OrderByDescending(name => name)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<string?> ReadLogFileAsync(string fileName)
    {
        // Защита от выхода за пределы каталога логов
        if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
        {
            return null;
        }

        var fullPath = Path.Combine(_logDirectory, fileName);
        if (!File.Exists(fullPath))
        {
            return null;
        }

        return await File.ReadAllTextAsync(fullPath, Encoding.UTF8);
    }
}
