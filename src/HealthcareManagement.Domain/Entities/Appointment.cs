using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.Enums;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;

namespace HealthcareManagement.Domain.Entities;

public sealed class Appointment : Entity
{
    protected Appointment() { }

    public Appointment(Guid id, Guid doctorId, Guid patientId, AppointmentDate appointmentDate) : base(id)
    {
        SetDomainProperties(doctorId, patientId, appointmentDate);
    }

    private readonly int appointmentDuration = 30;
    private readonly IList<Prescription> _prescriptions = new List<Prescription>();
    public Guid DoctorId { get; private set; }
    public Guid PatientId { get; private set; }
    public AppointmentDate AppointmentDate { get; private set; }
    public EAppointmentStatus Status { get; private set; }
    public IReadOnlyCollection<Prescription> Prescriptions => _prescriptions.AsReadOnly();


    public void Cancel(IDateTimeProvider dateTimeProvider)
    {
        var cancellationRequestDateTime = dateTimeProvider.GetUtcNow();

        var timeDifference = AppointmentDate.Date - cancellationRequestDateTime;

        DomainValidationException.When(Status != EAppointmentStatus.Scheduled, "Only scheduled appointments can be canceled");
        DomainValidationException.When(AppointmentDate.Date <= cancellationRequestDateTime, 
            "Cannot cancel an appointment that has already started or finished.");
        DomainValidationException.When(timeDifference.TotalHours < 24,
            "The appointment cannot be canceled within 24 hours of the appointment time");

        Status = EAppointmentStatus.Cancelled;
    }

    public void Reschedule(AppointmentDate newDate)
    {
        DomainValidationException.When(Status != EAppointmentStatus.Scheduled, "Only scheduled appointments can be reschedule");

        AppointmentDate = newDate;
    }

    public void Complete(IDateTimeProvider dateTimeProvider)
    {
        var currentTime = dateTimeProvider.GetUtcNow();
        var appointmentEndTime = AppointmentDate.Date.AddMinutes(appointmentDuration);

        DomainValidationException.When(Status != EAppointmentStatus.Scheduled, "Only scheduled appointments can be completed");
        DomainValidationException.When(AppointmentDate.Date > currentTime, "Cannot complete an appointment that has not started yet");
        DomainValidationException.When(currentTime < appointmentEndTime, "Cannot complete an appointment before it ends.");

        Status = EAppointmentStatus.Completed;
    }

    public void AddPrescription(Prescription prescription)
    {
        DomainValidationException.When(Status != EAppointmentStatus.Scheduled, "Only scheduled appointments can have prescriptions added");

        _prescriptions.Add(prescription);
    }

    public void UpdatePrescription(Guid prescriptionId,string medication, string dosage, string duration, string instructions)
    {
        DomainValidationException.When(Status != EAppointmentStatus.Scheduled, "Only scheduled appointments can have prescriptions added");

        var prescription = _prescriptions.FirstOrDefault(p => p.Id == prescriptionId);

        DomainValidationException.When(prescription is null, $"Prescription with ID {prescriptionId} not found");

        prescription.Update(
            medication, 
            dosage, 
            duration, 
            instructions);
    }

    private void SetDomainProperties(Guid doctorId, Guid patientId, AppointmentDate appointmentDate)
    {
        DoctorId = doctorId;
        PatientId = patientId;
        AppointmentDate = appointmentDate;
        Status = EAppointmentStatus.Scheduled;
    }

    public Doctor Doctor { get; set; }
    public Patient Patient { get; set; }
}
