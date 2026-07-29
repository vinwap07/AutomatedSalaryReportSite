using Domain.Abstractions;

namespace WebApp.Farms;

/// <summary>
/// Ферма текущего запроса. Заполняется из данных сессии в <see cref="Middlewares.SessionAuthMiddleware"/>
/// </summary>
public class FarmContext : IFarmContext
{
    /// <inheritdoc />
    public Guid? CurrentFarmId { get; private set; }

    /// <inheritdoc />
    public void SetFarm(Guid? farmId)
    {
        CurrentFarmId = farmId;
    }
}
