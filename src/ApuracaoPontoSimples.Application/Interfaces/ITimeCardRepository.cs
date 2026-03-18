using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.Interfaces;

public interface ITimeCardRepository
{
    Task<IReadOnlyList<TimeCard>> GetOverlappingAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
    Task AddAsync(TimeCard timeCard, CancellationToken cancellationToken);
    void RemoveRange(IEnumerable<TimeCard> timeCards);
}
