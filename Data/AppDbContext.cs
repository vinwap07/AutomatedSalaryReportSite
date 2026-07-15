using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Data;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Employee> Employees => Set<Employee>();
    
    public DbSet<Equipment> Equipments => Set<Equipment>();
    
    public DbSet<ReasonForAbsence> ReasonForAbsences => Set<ReasonForAbsence>();
    
    public DbSet<User> Users => Set<User>();
    
    public DbSet<WorkLog> WorkLogs => Set<WorkLog>();
    
    public DbSet<WorkType> WorkTypes => Set<WorkType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}