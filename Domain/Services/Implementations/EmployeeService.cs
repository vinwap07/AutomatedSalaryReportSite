using Domain.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Mappers;
using Domain.Repositories;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class EmployeeService(
    IGenericRepository<Employee, Guid> employeeRepository,
    IUnitOfWork unitOfWork
    ) : IEmployeeService
{
    public async Task<EmployeeDetailsDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = request.ToEmployee();
        employee.Id = Guid.NewGuid();
        await employeeRepository.AddAsync(request.ToEmployee(), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return employee.ToDetailsDto();
    }

    public async Task<EmployeeDetailsDto> UpdateAsync(UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException(nameof(Employee), request.Id);
        }
        request.UpdateEmployee(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return employee.ToDetailsDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await employeeRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<EmployeeDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException(nameof(Employee), id);
        }
        return employee.ToDetailsDto();
    }

    public async Task<IEnumerable<EmployeeListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetAllAsync(cancellationToken);
        return employees.Select(e => e.ToListItemDto());
    }

    public async Task<IEnumerable<EmployeeListItemDto>> GetByFiltersAsync(EmployeeFilters filters, CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.FindAsync(e => 
                (filters.Name == null || e.Name.Contains(filters.Name)) &&
                (filters.Specialty == null || e.Specialty == filters.Specialty) &&
                (filters.EquipmentId == null || e.EquipmentId == filters.EquipmentId),
            filters.Page,
            filters.PageSize,
            cancellationToken);
        return employees.Select(e => e.ToListItemDto());
    }
}