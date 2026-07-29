namespace WebApp.Options;

/// <summary>
/// Описание фермы и ее администратора из конфигурации.
/// При запуске приложения фермы и их администраторы создаются, если их еще нет
/// </summary>
public class FarmSeedOptions
{
    public const string SectionName = "Farms";

    /// <summary>
    /// Уникальный идентификатор фермы. Менять нельзя: к нему привязаны все данные фермы
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название фермы
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Логин администратора фермы
    /// </summary>
    public string AdminLogin { get; set; } = string.Empty;

    /// <summary>
    /// Пароль администратора фермы (используется только при создании учетной записи)
    /// </summary>
    public string AdminPassword { get; set; } = string.Empty;
}
