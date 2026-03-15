namespace ApuracaoPontoSimples.Domain.Entities;

public sealed class Employer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
    public string? Address { get; set; }
}
