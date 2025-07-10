using HealthcareManagement.Domain.Exceptions.Base;

namespace HealthcareManagement.Domain.Exceptions.Prescription;

public class PrescriptionNotFoundException(string message) : NotFoundException(message)
{}
