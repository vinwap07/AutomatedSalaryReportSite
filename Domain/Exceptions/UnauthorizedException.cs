namespace Domain.Exceptions;

/// <summary>
/// Исключение, возникающее при ошибке авторизации.
/// </summary>
public class UnauthorizedException(string message) : Exception(message);