using System.Globalization;
using Data;
using Domain;
using Domain.Dtos;
using Domain.Entities;
using Domain.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddData(builder.Configuration);
builder.Services.AddDomain(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.Configure<CookieSessionOptions>(builder.Configuration.GetSection(CookieSessionOptions.SectionName));
builder.Services.AddSingleton<ISessionStore, InMemorySessionStore>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddSingleton<IAuditLogger, FileAuditLogger>();

var app = builder.Build();

await InitializeDatabaseAsync(app);

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

// Применяет миграции и создает администратора по умолчанию, если пользователей еще нет
static async Task InitializeDatabaseAsync(Microsoft.AspNetCore.Builder.WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
    var users = await userService.GetAllAsync();
    if (!users.Any())
    {
        var login = app.Configuration["DefaultAdmin:Login"] ?? "admin";
        var password = app.Configuration["DefaultAdmin:Password"] ?? "admin";
        await userService.CreateAsync(new CreateUserRequest
        {
            Login = login,
            Password = password,
            Role = Role.Admin,
        });
        app.Logger.LogInformation("Создан администратор по умолчанию с логином '{Login}'", login);
    }
}
