namespace ApuracaoPontoSimples.Domain.Enums;

public enum DayCode
{
    None = 0,
    CF, // Compensacao/Ferias (usado no VBA como CF)
    F,  // Falta
    FR, // Ferias
    AB, // Abono
    A,  // Atestado
    DS  // DSR/Descanso semanal
}
