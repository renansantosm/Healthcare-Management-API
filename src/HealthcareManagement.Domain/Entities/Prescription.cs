using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.Validation;

namespace HealthcareManagement.Domain.Entities;

public sealed class Prescription : Entity
{
    protected Prescription() { }

    public Prescription(Guid id, Guid appointmentId, string medication, string dosage, string duration, string instructions) : base(id)
    {
        ValidateAppointmentId(appointmentId);
        ValidatePrescriptionDetails(medication, dosage, duration, instructions);
        SetDomainProperties(appointmentId, medication, dosage, duration, instructions);
    }

    public Guid AppointmentId { get; private set; }
    public string Medication { get; private set; }
    public string Dosage { get; private set; }
    public string Duration { get; private set; }
    public string Instructions { get; private set; }

    public void Update(string medication, string dosage, string duration, string instructions)
    {
        ValidatePrescriptionDetails(medication, dosage, duration, instructions);
        SetDomainPropertiesForUpdate(medication, dosage, duration, instructions);
    }

    private void ValidatePrescriptionDetails(string medication, string dosage, string duration, string instructions)
    {
        DomainValidationException.When(string.IsNullOrEmpty(medication), "Invalid medication value");
        DomainValidationException.When(medication.Length > 100, "Medication cannot exceed 100 characters");

        DomainValidationException.When(string.IsNullOrEmpty(dosage), "Invalid dosage value");
        DomainValidationException.When(dosage.Length > 50, "Dosage cannot exceed 50 characters");

        DomainValidationException.When(string.IsNullOrEmpty(duration), "Invalid duration value");
        DomainValidationException.When(duration.Length > 50, "Duration cannot exceed 50 characters");

        DomainValidationException.When(string.IsNullOrEmpty(instructions), "Invalid instructions value");
        DomainValidationException.When(instructions.Length > 500, "Instructions cannot exceed 500 characters");
    }

    private void ValidateAppointmentId(Guid appointmentId)
    {
        DomainValidationException.When(appointmentId == Guid.Empty, "Id cannot be empty");
    }

    private void SetDomainProperties(Guid appointmentId, string medication, string dosage, string duration, string instructions)
    {
        AppointmentId = appointmentId;
        Medication = medication;
        Dosage = dosage;
        Duration = duration;
        Instructions = instructions;
    }

    private void SetDomainPropertiesForUpdate(string medication, string dosage, string duration, string instructions)
    {
        Medication = medication;
        Dosage = dosage;
        Duration = duration;
        Instructions = instructions;
    }

    public Appointment Appointment { get; set; }
}
