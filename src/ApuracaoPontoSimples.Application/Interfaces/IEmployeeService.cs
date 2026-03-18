using ApuracaoPontoSimples.Application.Models;
using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.Interfaces;

public interface IEmployeeService
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken);
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ServiceResult<Employee>> CreateAsync(CreateEmployeeInput input, CancellationToken cancellationToken);
    Task<ServiceResult<Employee>> UpdateAsync(Guid id, UpdateEmployeeInput input, CancellationToken cancellationToken);
    Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
