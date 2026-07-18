using Domain.Dtos;
using Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Employees;

public class IndexModel(IEmployeeService employeeService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public EmployeeFilters Filters { get; set; } = new();
    
    public IEnumerable<EmployeeListItemDto> Employees { get; set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Employees = await employeeService.GetByFiltersAsync(Filters, cancellationToken);
    }
}