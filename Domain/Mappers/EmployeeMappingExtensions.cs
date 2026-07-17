using Domain.Dtos;
using Domain.Entities;
using Mapster;

namespace Domain.Mappers;

public static class EmployeeMappingExtensions
{
    public static EmployeeDetailsDto ToDetailsDto(this Employee employee)
    {
        return employee.Adapt<EmployeeDetailsDto>();
    }

    public static EmployeeListItemDto ToListItemDto(this Employee employee)
    {
        return employee.Adapt<EmployeeListItemDto>();
    }

    public static Employee ToEmployee(this CreateEmployeeRequest request)
    {
        return request.Adapt<Employee>();
    }

    public static void UpdateEmployee(this UpdateEmployeeRequest request, Employee existingEmployee)
    {
        request.Adapt(existingEmployee);
    }
}