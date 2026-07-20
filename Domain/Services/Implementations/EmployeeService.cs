using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Mappers;
using Domain.Repositories;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

/// <inheritdoc />
public class EmployeeService(
    IGenericRepository<Employee, Guid> employeeRepository,
    IUnitOfWork unitOfWork
    ) : IEmployeeService
{
    private static readonly string[] Includes =
    [
        nameof(Employee.Equipment),
        nameof(Employee.User),
    ];

    /// <inheritdoc />
    public async Task<EmployeeDetailsDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = request.ToEmployee();
        employee.Id = Guid.NewGuid();
        await employeeRepository.AddAsync(employee, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(employee.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EmployeeDetailsDto> UpdateAsync(UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException(nameof(Employee), request.Id);
        }
        request.UpdateEmployee(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(employee.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task LinkUserAsync(Guid employeeId, Guid? userId, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException(nameof(Employee), employeeId);
        }
        employee.UserId = userId;
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await employeeRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task<EmployeeDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.FindAsync(e => e.Id == id, 1, 1, cancellationToken, Includes);
        var employee = employees.FirstOrDefault();
        if (employee == null)
        {
            throw new NotFoundException(nameof(Employee), id);
        }
        return employee.ToDetailsDto();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<EmployeeListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetAllAsync(cancellationToken, Includes);
        return employees.Select(e => e.ToListItemDto());
    }

    /// <inheritdoc />
    public async Task<IEnumerable<EmployeeListItemDto>> GetByFiltersAsync(EmployeeFilters filters,
        CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.FindAsync(e =>
                (filters.Name == null || e.Name.Contains(filters.Name)) &&
                (filters.Specialty == null || e.Specialty == filters.Specialty) &&
                (filters.EquipmentId == null || e.EquipmentId == filters.EquipmentId) &&
                (filters.UserId == null || e.UserId == filters.UserId),
            filters.Page,
            filters.PageSize,
            cancellationToken,
            Includes);
        return employees.Select(e => e.ToListItemDto());
    }
}