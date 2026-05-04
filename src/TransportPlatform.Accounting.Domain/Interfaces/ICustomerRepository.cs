using TransportPlatform.Accounting.Domain.Entities;

namespace TransportPlatform.Accounting.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<Customer?> GetByIdentityIdAsync(Guid identityId, CancellationToken ct = default);
    Task<IEnumerable<Customer>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Customer customer, CancellationToken ct = default);
    Task UpdateAsync(Customer customer, CancellationToken ct = default);
}
