using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApuracaoPontoSimples.Infrastructure.Repositories;

public sealed class TimeCardRepository : ITimeCardRepository
{
    private readonly AppDbContext _db;

    public TimeCardRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TimeCard>> GetOverlappingAsync(
        Guid employeeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
        => await _db.TimeCards
            .Include(t => t.Days)
            .ThenInclude(d => d.Absence)
            .Where(t =>
                t.EmployeeId == employeeId &&
                t.StartDate <= endDate &&
                t.EndDate >= startDate)
            .ToListAsync(cancellationToken);

    public Task AddAsync(TimeCard timeCard, CancellationToken cancellationToken)
        => _db.TimeCards.AddAsync(timeCard, cancellationToken).AsTask();

    public void RemoveRange(IEnumerable<TimeCard> timeCards) => _db.TimeCards.RemoveRange(timeCards);
}
