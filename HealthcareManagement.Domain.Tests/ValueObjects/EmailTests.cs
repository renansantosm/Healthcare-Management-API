using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;

namespace HealthcareManagement.Domain.Tests.ValueObjects;

public class EmailTests
{
    private readonly string _validEmail = "john@email.com";
    private readonly string _validEmail2 = "doe@email.com";
    private readonly string _invalidEmail = "johnemail.com";
    private readonly string _invalidEmailMaxLength = new string('a', 101);

    [Fact]
    public void CreateEmail_WithValidData_ShouldCreateEmail()
    {
        var email = Email.Create(_validEmail);

        Assert.NotNull(email);
        Assert.Equal(_validEmail, email.Adress);
    }

    [Fact]
    public void CreateEmail_WithInvalidData_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => Email.Create(string.Empty));

        Assert.Equal("Email is required", exception.Message);
    }

    [Fact]
    public void CreateEmail_WithInvalidEmail_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => Email.Create(_invalidEmail));

        Assert.Equal("Invalid email format", exception.Message);
    }

    [Fact]
    public void CreateEmail_WithInvalidEmailLength_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => Email.Create(_invalidEmailMaxLength));

        Assert.Equal("Email cannot exceed 100 characters", exception.Message);
    }

    [Fact]
    public void Equals_WithSameEmail_ShouldReturnTrue()
    {
        var email1 = Email.Create(_validEmail);
        var email2 = Email.Create(_validEmail);

        Assert.True(email1.Equals(email2));
    }

    [Fact]
    public void Equals_WithDifferentEmail_ShouldReturnFalse()
    {
        var email1 = Email.Create(_validEmail);
        var email2 = Email.Create(_validEmail2);

        Assert.False(email1.Equals(email2));
    }
}
