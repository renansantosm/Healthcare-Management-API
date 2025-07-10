namespace HealthcareManagement.Domain.Interfaces;

public interface IDateTimeProvider
{
    DateTimeOffset GetUtcNow();
}
