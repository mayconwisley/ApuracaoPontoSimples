using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApuracaoPontoSimples.Infrastructure.Repositories;

public sealed class EmployerRepository : IEmployerRepository
{
    private readonly AppDbContext _db;

    public EmployerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Employer>> GetAllAsync(CancellationToken cancellationToken)
        => await _db.Employers.AsNoTracking().ToListAsync(cancellationToken);

    public Task<Employer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _db.Employers.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        => _db.Employers.AnyAsync(e => e.Id == id, cancellationToken);

    public void Add(Employer employer) => _db.Employers.Add(employer);

    public void Remove(Employer employer) => _db.Employers.Remove(employer);
}
