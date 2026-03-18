using ApuracaoPontoSimples.Application.Models;
using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.Interfaces;

public interface IEmployerService
{
    Task<IReadOnlyList<Employer>> GetAllAsync(CancellationToken cancellationToken);
    Task<ServiceResult<Employer>> CreateAsync(CreateEmployerInput input, CancellationToken cancellationToken);
    Task<ServiceResult<Employer>> UpdateAsync(Guid id, UpdateEmployerInput input, CancellationToken cancellationToken);
    Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
