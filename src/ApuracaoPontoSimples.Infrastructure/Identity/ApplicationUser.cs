using Microsoft.AspNetCore.Identity;

namespace ApuracaoPontoSimples.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string? FullName { get; set; }
}
