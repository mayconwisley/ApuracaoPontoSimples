using ApuracaoPontoSimples.Application.Models;
using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.Interfaces;

public interface ITimeCardService
{
    Task<ServiceResult<TimeCard>> CreateAsync(CreateTimeCardInput input, CancellationToken cancellationToken);
    Task<ServiceResult<TimeCard>> GetAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
}
