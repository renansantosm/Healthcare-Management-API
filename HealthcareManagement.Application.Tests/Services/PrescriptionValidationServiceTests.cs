using HealthcareManagement.Application.Services.Validation;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Prescription;
using HealthcareManagement.Domain.Interfaces;
using NSubstitute;

namespace HealthcareManagement.Application.Tests.Services;

public class PrescriptionValidationServiceTests
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly PrescriptionValidationService _service;

    public PrescriptionValidationServiceTests()
    {
        _prescriptionRepository = Substitute.For<IPrescriptionRepository>();
        _service = new PrescriptionValidationService(_prescriptionRepository);
    }

    [Fact]
    public async Task GetPrescriptionOrThrowAsync_WhenPrescriptionExists_ReturnsPrescription()
    {
        // Arrange
        var prescriptionId = Guid.NewGuid();

        var expectedPrescription = new Prescription(prescriptionId, Guid.NewGuid(), "Medication", "Dosage", "Duration", "Instructions");

        _prescriptionRepository.GetByIdAsync(prescriptionId).Returns(Task.FromResult(expectedPrescription));

        // Act
        var result = await _service.GetPrescriptionOrThrowAsync(prescriptionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPrescription, result);
    }

    [Fact]
    public async Task GetPrescriptionOrThrowAsync_WhenPrescriptionDoesNotExist_ThrowsPrescriptionNotFoundException()
    {
        // Arrange
        var prescriptionId = Guid.NewGuid();
        _prescriptionRepository.GetByIdAsync(prescriptionId).Returns(Task.FromResult<Prescription>(null));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PrescriptionNotFoundException>(() => _service.GetPrescriptionOrThrowAsync(prescriptionId));
        Assert.Equal($"Prescription with ID {prescriptionId} not found.", exception.Message);
    }
}
