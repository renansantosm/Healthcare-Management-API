using HealthcareManagement.Domain.Exceptions.Base;

namespace HealthcareManagement.Domain.Exceptions.Doctor;

public class DoctorNotFoundException(string message) : NotFoundException(message)
{}
