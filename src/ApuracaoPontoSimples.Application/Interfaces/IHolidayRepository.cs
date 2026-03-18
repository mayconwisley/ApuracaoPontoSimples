using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.Interfaces;

public interface IHolidayRepository
{
    Task<IReadOnlyList<Holiday>> GetAllAsync(CancellationToken cancellationToken);
    Task<Holiday?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(Holiday holiday);
    void Remove(Holiday holiday);
}
