namespace TransportPlatform.Accounting.Application.Commands;

public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string Position,
    string Department,
    DateOnly HireDate,
    decimal Salary,
    Guid? IdentityId = null);

public record CreateEmployeeResult(Guid EmployeeId);
