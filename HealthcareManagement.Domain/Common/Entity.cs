using HealthcareManagement.Domain.Validation;

namespace HealthcareManagement.Domain.Common;

public abstract class Entity
{
    protected Entity() { }

    protected Entity(Guid id)
    {
        DomainValidationException.When(id == Guid.Empty, "Id cannot be empty");

        Id = id;
    }

    public Guid Id { get; private set; }
}
