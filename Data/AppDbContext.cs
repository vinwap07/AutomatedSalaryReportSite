using Domain.Abstractions;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Data;

/// <summary>
/// Контекст базы данных приложения.
/// Все данные, кроме списка ферм, автоматически ограничены фермой текущего пользователя
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options, IFarmContext farmContext)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Equipment> Equipments => Set<Equipment>();

    public DbSet<ReasonForAbsence> ReasonForAbsences => Set<ReasonForAbsence>();

    public DbSet<User> Users => Set<User>();

    public DbSet<WorkLog> WorkLogs => Set<WorkLog>();

    public DbSet<WorkType> WorkTypes => Set<WorkType>();

    public DbSet<Farm> Farms => Set<Farm>();

    /// <summary>
    /// Ферма текущего пользователя. Null — фильтрация по ферме отключена
    /// (вход в систему, миграции, инициализация)
    /// </summary>
    public Guid? CurrentFarmId => farmContext.CurrentFarmId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Фильтры вычисляются для каждого экземпляра контекста, поэтому обращаемся к свойству контекста
        modelBuilder.Entity<User>().HasQueryFilter(x => CurrentFarmId == null || x.FarmId == CurrentFarmId);
        modelBuilder.Entity<Employee>().HasQueryFilter(x => CurrentFarmId == null || x.FarmId == CurrentFarmId);
        modelBuilder.Entity<Equipment>().HasQueryFilter(x => CurrentFarmId == null || x.FarmId == CurrentFarmId);
        modelBuilder.Entity<WorkType>().HasQueryFilter(x => CurrentFarmId == null || x.FarmId == CurrentFarmId);
        modelBuilder.Entity<ReasonForAbsence>().HasQueryFilter(x => CurrentFarmId == null || x.FarmId == CurrentFarmId);
        modelBuilder.Entity<WorkLog>().HasQueryFilter(x => CurrentFarmId == null || x.FarmId == CurrentFarmId);

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyFarmScope();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyFarmScope();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Проставляет ферму новым записям и запрещает переносить существующие записи в другую ферму
    /// </summary>
    private void ApplyFarmScope()
    {
        foreach (var entry in ChangeTracker.Entries<IFarmScoped>())
        {
            switch (entry.State)
            {
                case EntityState.Added when entry.Entity.FarmId == Guid.Empty:
                    entry.Entity.FarmId = CurrentFarmId
                        ?? throw new InvalidOperationException(
                            $"Невозможно сохранить \"{entry.Entity.GetType().Name}\": ферма не определена");
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(IFarmScoped.FarmId)).IsModified = false;
                    break;
            }
        }
    }
}
