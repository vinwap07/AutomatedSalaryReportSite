using System.Globalization;
using Data;
using Domain;
using Domain.Abstractions;
using Domain.Dtos;
using Domain.Entities;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using WebApp.Farms;
using WebApp.Logging;
using WebApp.Middlewares;
using WebApp.ModelBinding;
using WebApp.Options;
using WebApp.Session.Service;
using WebApp.Session.Store;

var culture = CultureInfo.GetCultureInfo("ru-RU");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new FlexibleDecimalModelBinderProvider()));

// Регистрируется до AddData: контекст базы данных фильтрует записи по ферме текущего пользователя
builder.Services.AddScoped<IFarmContext, FarmContext>();

builder.Services.AddData(builder.Configuration);
builder.Services.AddDomain(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.Configure<CookieSessionOptions>(builder.Configuration.GetSection(CookieSessionOptions.SectionName));
builder.Services.AddSingleton<ISessionStore, InMemorySessionStore>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddSingleton<IAuditLogger, FileAuditLogger>();

// Приложение работает за reverse-proxy (nginx): берем IP клиента и схему из заголовков X-Forwarded-*
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

await InitializeDatabaseAsync(app);

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseMiddleware<SessionAuthMiddleware>();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();
return;

// Применяет миграции и создает описанные в конфигурации фермы вместе с их администраторами
static async Task InitializeDatabaseAsync(Microsoft.AspNetCore.Builder.WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    var farms = app.Configuration.GetSection(FarmSeedOptions.SectionName).Get<List<FarmSeedOptions>>() ?? [];
    if (farms.Count == 0)
    {
        app.Logger.LogWarning("В конфигурации не описано ни одной фермы (секция '{Section}')", FarmSeedOptions.SectionName);
        return;
    }

    var farmService = scope.ServiceProvider.GetRequiredService<IFarmService>();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

    // Инициализация выполняется без контекста фермы, поэтому видны учетные записи всех ферм
    var logins = (await userService.GetAllAsync())
        .Select(u => u.Login)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    foreach (var farm in farms)
    {
        if (farm.Id == Guid.Empty || string.IsNullOrWhiteSpace(farm.Name))
        {
            throw new InvalidOperationException(
                $"Некорректное описание фермы в конфигурации '{FarmSeedOptions.SectionName}': нужны Id и Name");
        }

        await farmService.EnsureAsync(farm.Id, farm.Name.Trim());

        var login = farm.AdminLogin.Trim();
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(farm.AdminPassword))
        {
            throw new InvalidOperationException(
                $"Для фермы '{farm.Name}' не заданы логин и пароль администратора");
        }

        if (!logins.Add(login))
        {
            continue;
        }

        await userService.CreateAsync(new CreateUserRequest
        {
            Login = login,
            Password = farm.AdminPassword,
            Role = Role.Admin,
            FarmId = farm.Id,
        });
        app.Logger.LogInformation("Создан администратор '{Login}' фермы '{Farm}'", login, farm.Name);
    }
}
