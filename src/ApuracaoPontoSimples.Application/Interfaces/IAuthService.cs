using ApuracaoPontoSimples.Application.Models;

namespace ApuracaoPontoSimples.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<string>> RegisterAsync(RegisterInput input, CancellationToken cancellationToken);
    Task<ServiceResult<string>> LoginAsync(LoginInput input, CancellationToken cancellationToken);
    Task<ServiceResult<string>> BootstrapAsync(BootstrapInput input, CancellationToken cancellationToken);
}
