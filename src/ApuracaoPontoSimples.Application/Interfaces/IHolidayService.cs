using ApuracaoPontoSimples.Application.Models;
using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.Interfaces;

public interface IHolidayService
{
    Task<IReadOnlyList<Holiday>> GetAllAsync(CancellationToken cancellationToken);
    Task<ServiceResult<Holiday>> CreateAsync(HolidayInput input, CancellationToken cancellationToken);
    Task<ServiceResult<Holiday>> UpdateAsync(Guid id, HolidayInput input, CancellationToken cancellationToken);
    Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
