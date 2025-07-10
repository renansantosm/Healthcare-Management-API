using HealthcareManagement.Domain.Exceptions.Base;

namespace HealthcareManagement.Domain.Exceptions.Appointment;

public class AppointmentNotFoundException(string message) : NotFoundException(message)
{}
