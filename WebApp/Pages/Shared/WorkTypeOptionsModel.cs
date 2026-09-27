using Domain.Entities;

namespace WebApp.Pages.Shared;

/// <summary>
/// Пункты выпадающего списка видов работ, сгруппированные по специальностям
/// </summary>
/// <param name="Groups">Группы видов работ (см. SpecialtyExtensions.GroupBySpecialty)</param>
/// <param name="SelectedId">Выбранный вид работы</param>
public record WorkTypeOptionsModel(IReadOnlyList<IGrouping<Specialty?, WorkType>> Groups, Guid? SelectedId);
