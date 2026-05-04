namespace TransportPlatform.Accounting.Application.Queries;

public record GetCustomersQuery(int Page = 1, int PageSize = 20);

public record GetCustomerByIdQuery(Guid CustomerId);

public record CustomerDto(
    Guid Id,
    Guid? IdentityId,
    string FirstName,
    string LastName,
    string FullName,
    DateOnly DateOfBirth,
    string Email,
    DateTimeOffset CreatedAt);
