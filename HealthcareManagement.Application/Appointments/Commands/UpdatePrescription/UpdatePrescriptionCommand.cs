using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.UpdatePrescription;

public record UpdatePrescriptionCommand(
    Guid AppointmentId,
    Guid PrescriptionId, 
    string Medication, 
    string Dosage, 
    string Duration, 
    string Instructions) : IRequest<Unit>;

