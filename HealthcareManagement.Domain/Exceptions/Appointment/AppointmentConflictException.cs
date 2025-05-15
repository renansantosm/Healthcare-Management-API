using HealthcareManagement.Domain.Exceptions.Base;

namespace HealthcareManagement.Domain.Exceptions.Appointment;

public class AppointmentConflictException(string message) : ConflictException(message)
{}
