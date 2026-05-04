using TransportPlatform.Accounting.Application.Queries;
using TransportPlatform.Accounting.Domain.Exceptions;
using TransportPlatform.Accounting.Domain.Interfaces;

namespace TransportPlatform.Accounting.Application.Handlers;

public class GetEmployeesHandler(IEmployeeRepository employees)
{
    public async Task<PagedResult<EmployeeDto>> HandleAsync(
        GetEmployeesQuery query,
        CancellationToken ct = default)
    {
        var list = await employees.GetAllAsync(query.Page, query.PageSize, ct);
        var dtos = list.Select(e => new EmployeeDto(
            e.Id, e.IdentityId, e.FirstName, e.LastName, e.FullName,
            e.DateOfBirth, e.Email, e.Position, e.Department,
            e.HireDate, e.Salary, e.CreatedAt));

        return new PagedResult<EmployeeDto>(dtos, query.Page, query.PageSize, dtos.Count());
    }

    public async Task<EmployeeDto> HandleByIdAsync(
        GetEmployeeByIdQuery query,
        CancellationToken ct = default)
    {
        var employee = await employees.GetByIdAsync(query.EmployeeId, ct)
            ?? throw new EmployeeNotFoundException(query.EmployeeId);

        return new EmployeeDto(
            employee.Id, employee.IdentityId, employee.FirstName, employee.LastName, employee.FullName,
            employee.DateOfBirth, employee.Email, employee.Position, employee.Department,
            employee.HireDate, employee.Salary, employee.CreatedAt);
    }
}
