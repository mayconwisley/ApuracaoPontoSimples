namespace ApuracaoPontoSimples.Application.Models;

public sealed record CreateEmployerInput(string Name, string? Cnpj, string? Address);
public sealed record UpdateEmployerInput(string Name, string? Cnpj, string? Address);
