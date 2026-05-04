using TransportPlatform.Accounting.Domain.Entities;

namespace TransportPlatform.Accounting.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Employee>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Employee employee, CancellationToken ct = default);
}
