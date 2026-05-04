using TransportPlatform.Accounting.Application.Queries;
using TransportPlatform.Accounting.Domain.Exceptions;
using TransportPlatform.Accounting.Domain.Interfaces;

namespace TransportPlatform.Accounting.Application.Handlers;

public class GetCustomersHandler(ICustomerRepository customers)
{
    public async Task<PagedResult<CustomerDto>> HandleAsync(
        GetCustomersQuery query,
        CancellationToken ct = default)
    {
        var list = await customers.GetAllAsync(query.Page, query.PageSize, ct);
        var dtos = list.Select(c => new CustomerDto(
            c.Id, c.IdentityId, c.FirstName, c.LastName, c.FullName,
            c.DateOfBirth, c.Email, c.CreatedAt));

        return new PagedResult<CustomerDto>(dtos, query.Page, query.PageSize, dtos.Count());
    }

    public async Task<CustomerDto> HandleByIdAsync(
        GetCustomerByIdQuery query,
        CancellationToken ct = default)
    {
        var customer = await customers.GetByIdAsync(query.CustomerId, ct)
            ?? throw new CustomerNotFoundException(query.CustomerId);

        return new CustomerDto(
            customer.Id, customer.IdentityId, customer.FirstName, customer.LastName, customer.FullName,
            customer.DateOfBirth, customer.Email, customer.CreatedAt);
    }
}
