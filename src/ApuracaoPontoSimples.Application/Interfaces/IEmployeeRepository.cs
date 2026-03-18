using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllWithEmployerAsync(CancellationToken cancellationToken);
    Task<Employee?> GetByIdWithScheduleAsync(Guid id, CancellationToken cancellationToken);
    Task<Employee?> GetByIdWithScheduleReadOnlyAsync(Guid id, CancellationToken cancellationToken);
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(Employee employee);
    void Remove(Employee employee);
}
