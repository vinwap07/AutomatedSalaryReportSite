using Domain.Entities;
using Domain.Services.Abstractions;
using Domain.Services.Implementations;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWorkLogService, WorkLogService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IReasonForAbsenceService, ReasonForAbsenceService>();
        services.AddScoped<IWorkTypeService, WorkTypeService>();
        
        return services;
    }
}