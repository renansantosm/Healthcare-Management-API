using HealthcareManagement.Domain.Common;
using HealthcareManagement.Domain.ValueObjects;

namespace HealthcareManagement.Domain.Entities;

public sealed class Patient : Entity
{
    protected Patient() { }

    public Patient(Guid id, PersonInfo info) : base(id)
    {
        SetDomainProperties(info);
    }

    private readonly IList<Appointment> _appointments = new List<Appointment>();
    public PersonInfo PersonInfo { get; private set; }
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    public void Update(PersonInfo info)
    {
        SetDomainProperties(info);
    }

    private void SetDomainProperties(PersonInfo info)
    {
        PersonInfo = info;
    }
}
