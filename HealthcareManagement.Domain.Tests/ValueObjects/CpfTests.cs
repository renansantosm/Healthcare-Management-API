using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;

namespace HealthcareManagement.Domain.Tests.ValueObjects;

public class CpfTests
{
    private readonly string _validCpf = "12345678909";
    private readonly string _validCpf2 = "12345678900";
    private readonly string _invalidCpf = "123456789";
    private readonly string _invalidCpfWithLetters = "1234567890a";

    [Fact]
    public void CreateCpf_WithValidData_ShouldCreateCpf()
    {
        var cpf = Cpf.Create(_validCpf);

        Assert.NotNull(cpf);
        Assert.Equal(_validCpf, cpf.Number);
    }

    [Fact]
    public void CreateCpf_WithInvalidData_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => Cpf.Create(string.Empty));

        Assert.Equal("CPF is required", exception.Message);
    }

    [Fact]
    public void CreateCpf_WithInvalidLength_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => Cpf.Create(_invalidCpf));

        Assert.Equal("CPF must have exactly 11 digits.", exception.Message);
    }

    [Fact]
    public void CreateCpf_WithLetters_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => Cpf.Create(_invalidCpfWithLetters));

        Assert.Equal("CPF must contain only numbers", exception.Message);
    }

    [Fact]
    public void Equals_WithSameCpf_ShouldReturnTrue()
    {
        var cpf1 = Cpf.Create(_validCpf);
        var cpf2 = Cpf.Create(_validCpf);

        Assert.True(cpf1.Equals(cpf2));
    }

    [Fact]
    public void Equals_WithDifferentCpf_ShouldReturnFalse()
    {
        var cpf1 = Cpf.Create(_validCpf);
        var cpf2 = Cpf.Create(_validCpf2);

        Assert.False(cpf1.Equals(cpf2));
    }
}
