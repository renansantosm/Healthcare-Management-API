using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.AddPrescription;

public record AddPrescriptionCommand(
    Guid AppointmentId, 
    string Medication, 
    string Dosage, 
    string Duration, 
    string Instructions) : IRequest<Guid>;
