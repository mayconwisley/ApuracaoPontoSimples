namespace ApuracaoPontoSimples.Api.Contracts;

public sealed record RegisterRequest(string Email, string Password, string FullName, string Role);
public sealed record LoginRequest(string Email, string Password);
public sealed record AuthResponse(string Token);
public sealed record BootstrapRequest(string Email, string Password, string FullName);
