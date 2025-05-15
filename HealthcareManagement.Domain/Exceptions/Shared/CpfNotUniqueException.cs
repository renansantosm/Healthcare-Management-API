using HealthcareManagement.Domain.Exceptions.Base;

namespace HealthcareManagement.Domain.Exceptions.Shared;

public class CpfNotUniqueException(string message) : ConflictException(message)
{}
