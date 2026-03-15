namespace ApuracaoPontoSimples.Domain.Entities;

public sealed class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Pis { get; set; }
    public DateOnly? AdmissionDate { get; set; }
    public Guid EmployerId { get; set; }
    public Employer? Employer { get; set; }
    public ScheduleConfig? Schedule { get; set; }
}
