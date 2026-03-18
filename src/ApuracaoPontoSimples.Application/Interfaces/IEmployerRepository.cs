using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.Interfaces;

public interface IEmployerRepository
{
    Task<IReadOnlyList<Employer>> GetAllAsync(CancellationToken cancellationToken);
    Task<Employer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    void Add(Employer employer);
    void Remove(Employer employer);
}
