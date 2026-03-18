namespace ApuracaoPontoSimples.Application.Models;

public sealed record RegisterInput(string Email, string Password, string FullName, string Role);
public sealed record LoginInput(string Email, string Password);
public sealed record BootstrapInput(string Email, string Password, string FullName);
