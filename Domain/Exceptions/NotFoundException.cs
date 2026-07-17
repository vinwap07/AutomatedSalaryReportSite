namespace Domain.Exceptions;

/// <summary>
/// Исключение, возникающее, когда сущность не найдена.
/// </summary>
public class NotFoundException(
    string name,
    object key
    ) : Exception($"Сущность \"{name}\" ({key}) не найдена.");