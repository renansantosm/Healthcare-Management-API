using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HealthcareManagement.Infra.Data.Repositories;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly AppDbContext _context;

    public PrescriptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Prescription>> GetPrescriptionsAsync(int pageNumber, int pageSize)
    {
        return await _context.Prescriptions
            .AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<Prescription?> GetByIdAsync(Guid id)
    {
        return await _context.Prescriptions.FirstOrDefaultAsync(p => p.Id == id);
    }
}
