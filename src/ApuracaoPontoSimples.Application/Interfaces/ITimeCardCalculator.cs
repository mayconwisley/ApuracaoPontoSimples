using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.Interfaces;

public interface ITimeCardCalculator
{
    TimeCard Calculate(TimeCard timeCard, ScheduleConfig schedule);
}
