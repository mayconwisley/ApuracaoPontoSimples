namespace ApuracaoPontoSimples.Domain.Entities;

public sealed class Holiday
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public DateOnly Date { get; set; }
	public string Description { get; set; } = string.Empty;
}
