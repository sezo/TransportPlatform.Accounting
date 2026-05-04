namespace TransportPlatform.Accounting.Application.Queries;

public record GetEmployeesQuery(int Page = 1, int PageSize = 20);

public record GetEmployeeByIdQuery(Guid EmployeeId);

public record EmployeeDto(
    Guid Id,
    Guid? IdentityId,
    string FirstName,
    string LastName,
    string FullName,
    DateOnly DateOfBirth,
    string Email,
    string Position,
    string Department,
    DateOnly HireDate,
    decimal Salary,
    DateTimeOffset CreatedAt);
