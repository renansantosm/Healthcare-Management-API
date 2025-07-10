using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using HealthcareManagement.Infra.Data.Context;
using HealthcareManagement.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HealthcareManagement.Infra.Data.Tests.Repositories;

public class DoctorRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _dbContext;
    private readonly IDoctorRepository _doctorRepository;
    private readonly Guid _doctorId = Guid.NewGuid();
    private readonly string _existingCpf = "12345678901";
    private readonly string _existingEmail = "john_doe@email.com";
    private readonly string _nonExistingCpf = "98765432101";
    private readonly string _nonExistingEmail = "nonexisting@email.com";

    public DoctorRepositoryTests()
    {
        // Um banco em memória com nome único (GUID) garante isolamento entre testes
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"DoctorRepositoryTests_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AppDbContext(_options);
        _doctorRepository = new DoctorRepository(_dbContext);
    }

    private async Task SeedDatabaseAsync()
    {
        var doctor = new Doctor(
            _doctorId,
            PersonInfo.Create(
                FullName.Create("John", "Doe"),
                BirthDate.Create(new DateOnly(1990, 10, 21)),
                Cpf.Create(_existingCpf),
                Email.Create(_existingEmail),
                MobilePhoneNumber.Create("11987654321")));


        _dbContext.Doctors.Add(doctor);
        await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task CpfExistsAsync_WithExistingCpf_ReturnsTrue()
    {
        // Arrange
        await SeedDatabaseAsync(); 

        // Act
        var result = await _doctorRepository.CpfExistsAsync(_existingCpf);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CpfExistsAsync_WithNonExistingCpf_ReturnsFalse()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var result = await _doctorRepository.CpfExistsAsync(_nonExistingCpf);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithExistingEmail_ReturnsTrue()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var result = await _doctorRepository.EmailExistsAsync(_existingEmail);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithNonExistingEmail_ReturnsFalse()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var result = await _doctorRepository.EmailExistsAsync(_nonExistingEmail);

        // Assert
        Assert.False(result);
    }
}
