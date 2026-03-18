using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApuracaoPontoSimples.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Employee>> GetAllWithEmployerAsync(CancellationToken cancellationToken)
        => await _db.Employees
            .Include(e => e.Employer)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<Employee?> GetByIdWithScheduleAsync(Guid id, CancellationToken cancellationToken)
        => await _db.Employees
            .Include(e => e.Schedule)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Employee?> GetByIdWithScheduleReadOnlyAsync(Guid id, CancellationToken cancellationToken)
        => await _db.Employees
            .Include(e => e.Schedule)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _db.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public void Add(Employee employee) => _db.Employees.Add(employee);

    public void Remove(Employee employee) => _db.Employees.Remove(employee);
}
