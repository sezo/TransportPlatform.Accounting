using TransportPlatform.Accounting.Domain.Exceptions;

namespace TransportPlatform.Accounting.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public Guid? IdentityId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    private Customer() { }

    public static Customer Create(
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string email,
        Guid? identityId = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new AccountingDomainException("Customer first name is required.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new AccountingDomainException("Customer last name is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new AccountingDomainException("Customer email is required.");
        if (dateOfBirth >= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-16))
            throw new AccountingDomainException("Customer must be at least 16 years old.");

        return new Customer
        {
            Id = Guid.NewGuid(),
            IdentityId = identityId,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            DateOfBirth = dateOfBirth,
            Email = email.Trim().ToLowerInvariant(),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Update(string firstName, string lastName, DateOnly dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new AccountingDomainException("Customer first name is required.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new AccountingDomainException("Customer last name is required.");
        if (dateOfBirth >= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-16))
            throw new AccountingDomainException("Customer must be at least 16 years old.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;
    }

    public string FullName => $"{FirstName} {LastName}";
}
