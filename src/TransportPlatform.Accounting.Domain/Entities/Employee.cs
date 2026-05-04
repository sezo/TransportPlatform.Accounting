using TransportPlatform.Accounting.Domain.Exceptions;

namespace TransportPlatform.Accounting.Domain.Entities;

public class Employee
{
    public Guid Id { get; private set; }
    public Guid? IdentityId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Position { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public DateOnly HireDate { get; private set; }
    public decimal Salary { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Employee() { }

    public static Employee Create(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string email,
        string position,
        string department,
        DateOnly hireDate,
        decimal salary,
        Guid? identityId = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new AccountingDomainException("Employee first name is required.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new AccountingDomainException("Employee last name is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new AccountingDomainException("Employee email is required.");
        if (string.IsNullOrWhiteSpace(position))
            throw new AccountingDomainException("Employee position is required.");
        if (string.IsNullOrWhiteSpace(department))
            throw new AccountingDomainException("Employee department is required.");
        if (salary < 0)
            throw new AccountingDomainException("Employee salary cannot be negative.");

        return new Employee
        {
            Id = Guid.NewGuid(),
            IdentityId = identityId,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            DateOfBirth = dateOfBirth,
            Email = email.Trim().ToLowerInvariant(),
            Position = position.Trim(),
            Department = department.Trim(),
            HireDate = hireDate,
            Salary = salary,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public string FullName => $"{FirstName} {LastName}";
}
