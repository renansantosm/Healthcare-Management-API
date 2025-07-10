using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Validation;

namespace HealthcareManagement.Domain.Tests.Entities;

public class PrescriptionsTests
{
    private readonly Guid _guid = Guid.Parse("d2719f5a-3c40-4c2e-9c75-9d9a3b6bfb1d");
    private readonly Guid _appointmentGuid = Guid.Parse("d2718f5a-2c39-4c2e-9c75-9d9a3b6bfb1d");
    private readonly Guid _emptyGuid = Guid.Empty;
    private readonly string _medication = "Paracetamol";
    private readonly string _dosage = "1 comprimido";
    private readonly string _duration = "5 dias";
    private readonly string _instructions = "Tomar a cada 8 horas";
    private readonly string _longMedication = new string('a', 101);
    private readonly string _longDosageAndDuration = new string('a', 51);
    private readonly string _longInstructions = new string('a', 501);

    [Fact]
    public void CreatePrescription_ValidData_ShouldCreatePrescription()
    {
        // Arrange
        var prescription = new Prescription(_guid, _appointmentGuid, _medication, _dosage, _duration, _instructions);

        // Act
        var result = prescription;

        // Assert
        Assert.Equal(_guid, result.Id);
        Assert.Equal(_appointmentGuid, result.AppointmentId);
        Assert.Equal(_medication, result.Medication);
        Assert.Equal(_dosage, result.Dosage);
        Assert.Equal(_duration, result.Duration);
        Assert.Equal(_instructions, result.Instructions);
    }

    [Fact]
    public void CreatePrescription_WithEmptyId_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Prescription(_emptyGuid, _appointmentGuid, _medication, _dosage, _duration, _instructions));

        Assert.Equal("Id cannot be empty", exception.Message);
    }

    [Fact]
    public void CreatePrescription_WithEmptyAppointmentId_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Prescription(_guid, _emptyGuid, _medication, _dosage, _duration, _instructions));

        Assert.Equal("Id cannot be empty", exception.Message);
    }

    [Fact]
    public void CreatePrescription_WithInvalidMedication_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Prescription(_guid, _appointmentGuid, string.Empty, _dosage, _duration, _instructions));

        Assert.Equal("Invalid medication value", exception.Message);
    }

    [Fact]
    public void CreatePrescription_WithLongMedication_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Prescription(_guid, _appointmentGuid, _longMedication, _dosage, _duration, _instructions));

        Assert.Equal("Medication cannot exceed 100 characters", exception.Message);
    }

    [Fact]
    public void CreatePrescription_WithInvalidDosage_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Prescription(_guid, _appointmentGuid, _medication, string.Empty, _duration, _instructions));

        Assert.Equal("Invalid dosage value", exception.Message);
    }

    [Fact]
    public void CreatePrescription_WithLongDosage_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Prescription(_guid, _appointmentGuid, _medication, _longDosageAndDuration, _duration, _instructions));

        Assert.Equal("Dosage cannot exceed 50 characters", exception.Message);
    }

    [Fact]
    public void CreatePrescription_WithInvalidDuration_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Prescription(_guid, _appointmentGuid, _medication, _dosage, string.Empty, _instructions));

        Assert.Equal("Invalid duration value", exception.Message);
    }

    [Fact]
    public void CreatePrescription_WithLongDuration_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Prescription(_guid, _appointmentGuid, _medication, _dosage, _longDosageAndDuration, _instructions));

        Assert.Equal("Duration cannot exceed 50 characters", exception.Message);
    }

    [Fact]
    public void CreatePrescription_WithInvalidInstructions_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Prescription(_guid, _appointmentGuid, _medication, _dosage, _duration, string.Empty));

        Assert.Equal("Invalid instructions value", exception.Message);
    }

    [Fact]
    public void CreatePrescription_WithLongInstructions_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Prescription(_guid, _appointmentGuid, _medication, _dosage, _duration, _longInstructions));

        Assert.Equal("Instructions cannot exceed 500 characters", exception.Message);
    }

    [Fact]
    public void UpdatePrescription_ValidData_ShouldUpdatePrescription()
    {
        // Arrange
        var prescription = new Prescription(_guid, _appointmentGuid, _medication, _dosage, _duration, _instructions);
        var newMedication = "Ibuprofeno";
        var newDosage = "1 comprimido";
        var newDuration = "7 dias";
        var newInstructions = "Tomar a cada 6 horas";

        // Act
        prescription.Update(newMedication, newDosage, newDuration, newInstructions);

        // Assert
        Assert.Equal(newMedication, prescription.Medication);
        Assert.Equal(newDosage, prescription.Dosage);
        Assert.Equal(newDuration, prescription.Duration);
        Assert.Equal(newInstructions, prescription.Instructions);
    }
}
