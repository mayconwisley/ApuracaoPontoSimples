using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApuracaoPontoSimples.Infrastructure.Repositories;

public sealed class HolidayRepository : IHolidayRepository
{
    private readonly AppDbContext _db;

    public HolidayRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Holiday>> GetAllAsync(CancellationToken cancellationToken)
        => await _db.Holidays.AsNoTracking().ToListAsync(cancellationToken);

    public Task<Holiday?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _db.Holidays.FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

    public void Add(Holiday holiday) => _db.Holidays.Add(holiday);

    public void Remove(Holiday holiday) => _db.Holidays.Remove(holiday);
}
