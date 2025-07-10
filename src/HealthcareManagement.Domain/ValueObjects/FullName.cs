using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.Validation;

namespace HealthcareManagement.Domain.ValueObjects;

public sealed class FullName : ValueObject<FullName>
{
    protected FullName() { }

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public const int MinLength = 3;
    public const int MaxLength = 100;
    public string FirstName { get; }
    public string LastName { get; }

    public static FullName Create(string firstName, string lastName)
    {
        DomainValidationException.When(string.IsNullOrEmpty(firstName), "First name is required");
        DomainValidationException.When(firstName.Length < MinLength, "Name must have at least 3 characters");
        DomainValidationException.When(firstName.Length > MaxLength, "Name must have a maximum of 100 characters");

        DomainValidationException.When(string.IsNullOrEmpty(lastName), "Last name is required");
        DomainValidationException.When(lastName.Length < MinLength, "Last name must have at least 3 characters");
        DomainValidationException.When(lastName.Length > MaxLength, "Last name must have a maximum of 100 characters");

        return new FullName(firstName, lastName);
    }


    protected override bool EqualsCore(FullName other)
    {
        return FirstName == other.FirstName
            && LastName == other.LastName;
    }

    protected override int GetHashCodeCore()
    {
        unchecked
        {
            int hashCode = FirstName.GetHashCode();
            hashCode = (hashCode * 397) ^ LastName.GetHashCode();
            return hashCode;
        }
    }
}
