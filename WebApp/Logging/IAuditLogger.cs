namespace WebApp.Logging;

/// <summary>
/// Текстовый журнал действий пользователей (создание/изменение/удаление записей)
/// </summary>
public interface IAuditLogger
{
    /// <summary>
    /// Записывает действие пользователя в текстовый лог
    /// </summary>
    /// <param name="login">Логин пользователя, выполнившего действие</param>
    /// <param name="action">Действие (например "Добавлена запись о работе")</param>
    /// <param name="details">Подробности действия</param>
    Task LogAsync(string login, string action, string details);

    /// <summary>
    /// Возвращает список имен файлов логов (новые первыми)
    /// </summary>
    IReadOnlyList<string> GetLogFileNames();

    /// <summary>
    /// Читает содержимое файла лога по имени (null, если файл не найден)
    /// </summary>
    Task<string?> ReadLogFileAsync(string fileName);
}
