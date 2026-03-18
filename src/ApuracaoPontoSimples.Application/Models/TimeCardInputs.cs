using ApuracaoPontoSimples.Domain.Enums;

namespace ApuracaoPontoSimples.Application.Models;

public sealed record DayEntryInput(
    DateOnly Date,
    DayCode Code,
    TimeSpan? Entrada1,
    TimeSpan? Saida1,
    TimeSpan? Entrada2,
    TimeSpan? Saida2,
    TimeSpan? Entrada3,
    TimeSpan? Saida3,
    HolidayType HolidayType,
    bool IsSunday,
    bool IsSaturday,
    TimeSpan? AbsenceHours,
    AbsenceType AbsenceType);

public sealed record CreateTimeCardInput(Guid EmployeeId, DateOnly StartDate, DateOnly EndDate, List<DayEntryInput> Days);
