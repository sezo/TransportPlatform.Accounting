using Microsoft.Extensions.Logging;
using TransportPlatform.Accounting.Application.Commands;
using TransportPlatform.Accounting.Domain.Entities;
using TransportPlatform.Accounting.Domain.Interfaces;

namespace TransportPlatform.Accounting.Application.Handlers;

public class CreateEmployeeHandler(
    IEmployeeRepository employees,
    ILogger<CreateEmployeeHandler> logger)
{
    public async Task<CreateEmployeeResult> HandleAsync(
        CreateEmployeeCommand command,
        CancellationToken ct = default)
    {
        var employee = Employee.Create(
            command.FirstName,
            command.LastName,
            command.DateOfBirth,
            command.Email,
            command.Position,
            command.Department,
            command.HireDate,
            command.Salary,
            command.IdentityId);

        await employees.AddAsync(employee, ct);

        logger.LogInformation("Employee {EmployeeId} created: {FullName} — {Position}",
            employee.Id, employee.FullName, employee.Position);

        return new CreateEmployeeResult(employee.Id);
    }
}
