using HealthcareManagement.Domain.Exceptions.Base;

namespace HealthcareManagement.Domain.Exceptions.Shared;

public class EmailNotUniqueException(string message) : ConflictException(message)
{}
