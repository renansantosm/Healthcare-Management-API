using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.Validation;

namespace HealthcareManagement.Domain.ValueObjects;

public sealed class Email : ValueObject<Email>
{
    protected Email() { }

    private Email(string adress)
    {
        Adress = adress;
    }

    private const int MaxLength = 100;
    public string Adress { get; }

    public static Email Create(string email)
    {
        DomainValidationException.When(string.IsNullOrEmpty(email), "Email is required");
        DomainValidationException.When(email.Length > MaxLength, "Email cannot exceed 100 characters");
        DomainValidationException.When(!email.Contains('@'), "Invalid email format");

        return new Email(email);
    }

    protected override bool EqualsCore(Email other)
    {
        return Adress == other.Adress;
    }

    protected override int GetHashCodeCore()
    {
        unchecked
        {
            int hashCode = 17;
            hashCode = (hashCode * 317) ^ Adress.GetHashCode();
            return hashCode;
        }
    }
}
