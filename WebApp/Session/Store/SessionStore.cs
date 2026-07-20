using System.Collections.Concurrent;
using WebApp.Session.Service;

namespace WebApp.Session.Store;

/// <summary>
/// Хранилище пользовательских сессий в памяти приложения
/// </summary>
public class InMemorySessionStore : ISessionStore
{
    private readonly ConcurrentDictionary<string, SessionDataDto> _sessions = new();

    /// <inheritdoc />
    public Task SaveAsync(string key, SessionDataDto sessionData, CancellationToken cancellationToken = default)
    {
        _sessions[key] = sessionData;
        CleanupExpired();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<SessionDataDto?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!_sessions.TryGetValue(key, out var sessionData))
        {
            return Task.FromResult<SessionDataDto?>(null);
        }

        if (sessionData.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            _sessions.TryRemove(key, out _);
            return Task.FromResult<SessionDataDto?>(null);
        }

        return Task.FromResult<SessionDataDto?>(sessionData);
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _sessions.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    private void CleanupExpired()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var (key, data) in _sessions)
        {
            if (data.ExpiresAt <= now)
            {
                _sessions.TryRemove(key, out _);
            }
        }
    }
}
