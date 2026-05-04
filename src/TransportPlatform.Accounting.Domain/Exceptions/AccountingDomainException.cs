namespace TransportPlatform.Accounting.Domain.Exceptions;

public class AccountingDomainException(string message) : Exception(message);

public class CustomerNotFoundException(Guid customerId)
    : Exception($"Customer {customerId} was not found.");

public class EmployeeNotFoundException(Guid employeeId)
    : Exception($"Employee {employeeId} was not found.");

public class LedgerEntryNotFoundException(Guid ticketId)
    : Exception($"Ledger entry for ticket {ticketId} was not found.");
