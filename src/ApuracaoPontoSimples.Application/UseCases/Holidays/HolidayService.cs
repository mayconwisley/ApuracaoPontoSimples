using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.UseCases.Holidays;

public sealed class HolidayService : IHolidayService
{
    private readonly IHolidayRepository _holidays;
    private readonly IUnitOfWork _unitOfWork;

    public HolidayService(IHolidayRepository holidays, IUnitOfWork unitOfWork)
    {
        _holidays = holidays;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyList<Holiday>> GetAllAsync(CancellationToken cancellationToken)
        => _holidays.GetAllAsync(cancellationToken);

    public async Task<ServiceResult<Holiday>> CreateAsync(HolidayInput input, CancellationToken cancellationToken)
    {
        var holiday = new Holiday { Date = input.Date, Description = input.Description };
        _holidays.Add(holiday);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult<Holiday>.Ok(holiday);
    }

    public async Task<ServiceResult<Holiday>> UpdateAsync(Guid id, HolidayInput input, CancellationToken cancellationToken)
    {
        var holiday = await _holidays.GetByIdAsync(id, cancellationToken);
        if (holiday == null)
            return ServiceResult<Holiday>.Fail(ServiceErrorType.NotFound, "Holiday not found.");

        holiday.Date = input.Date;
        holiday.Description = input.Description;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult<Holiday>.Ok(holiday);
    }

    public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var holiday = await _holidays.GetByIdAsync(id, cancellationToken);
        if (holiday == null)
            return ServiceResult.Fail(ServiceErrorType.NotFound, "Holiday not found.");

        _holidays.Remove(holiday);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult.Ok();
    }
}
