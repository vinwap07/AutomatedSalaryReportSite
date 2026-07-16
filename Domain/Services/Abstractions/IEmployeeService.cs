using Domain.Dtos;
using Domain.Entities;

namespace Domain.Services.Abstractions;

public interface IEmployeeService
{
    Task<EmployeeDetailsDto> GetEmployeeDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeListItemDto>> GetEmployeesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeListItemDto>> GetEmployeesWithFiltersAsync(EmployeeFilters filters, CancellationToken cancellationToken = default);
    Task<EmployeeDetailsDto> CreateEmployeeAsync(CreateEmployeeRequest employee, CancellationToken cancellationToken = default);
    Task<EmployeeDetailsDto> UpdateEmployeeAsync(UpdateEmployeeRequest employee, CancellationToken cancellationToken = default);
    Task DeleteEmployeeAsync(Guid id, CancellationToken cancellationToken = default);
}