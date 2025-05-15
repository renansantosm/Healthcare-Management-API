using HealthcareManagement.Domain.Exceptions.Base;

namespace HealthcareManagement.Domain.Exceptions.Shared;

public class PatientNotFoundException(string message) : NotFoundException(message)
{}
