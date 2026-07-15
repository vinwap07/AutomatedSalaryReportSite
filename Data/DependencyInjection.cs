using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Data;

public static class DependencyInjection
{
    /// <summary>
    /// Регистрирует необходимые зависимости для работы базы данных
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация проекта</param>
    /// <returns>Коллекция сервисов</returns>
    /// <exception cref="InvalidOperationException">Строка подключения к базе данных не была указана в конфигурации</exception>
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
        }
        
        services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUnitOfWork>(provider 
            => provider.GetRequiredService<AppDbContext>());
        
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        
        return services;
    }
}