using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.Validation;

namespace HealthcareManagement.Domain.ValueObjects;

public class Specialty : ValueObject<Specialty>
{
    protected Specialty() { }

    private Specialty(string name)
    {
        Name = name.Trim().ToLowerInvariant();
    }

    private const int MinLegnth = 3;
    private const int MaxLength = 100;
    public string Name { get; }

    public static Specialty Create(string name)
    {
        DomainValidationException.When(string.IsNullOrEmpty(name), "Specialty name is required");
        DomainValidationException.When(name.Length < MinLegnth, "Specialty name too short");
        DomainValidationException.When(name.Length > MaxLength, "Specialty name too long");

        return new Specialty(name);
    }

    protected override bool EqualsCore(Specialty other)
    {
        return Name == other.Name;
    }

    protected override int GetHashCodeCore()
    {
        unchecked
        {
            int hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ Name.GetHashCode();
            return hashCode;    
        }
    }
}
