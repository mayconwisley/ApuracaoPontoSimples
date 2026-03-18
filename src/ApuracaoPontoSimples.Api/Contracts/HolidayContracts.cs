using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Api.Contracts;

public sealed record HolidayRequest(DateOnly Date, string Description);
public sealed record HolidayDto(Guid Id, DateOnly Date, string Description);

public static class HolidayMappings
{
    public static HolidayDto ToDto(this Holiday holiday)
        => new(holiday.Id, holiday.Date, holiday.Description);
}
