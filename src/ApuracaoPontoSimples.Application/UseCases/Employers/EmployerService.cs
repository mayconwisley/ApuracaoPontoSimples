using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.UseCases.Employers;

public sealed class EmployerService : IEmployerService
{
    private readonly IEmployerRepository _employers;
    private readonly IUnitOfWork _unitOfWork;

    public EmployerService(IEmployerRepository employers, IUnitOfWork unitOfWork)
    {
        _employers = employers;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyList<Employer>> GetAllAsync(CancellationToken cancellationToken)
        => _employers.GetAllAsync(cancellationToken);

    public async Task<ServiceResult<Employer>> CreateAsync(CreateEmployerInput input, CancellationToken cancellationToken)
    {
        var employer = new Employer
        {
            Name = input.Name,
            Cnpj = input.Cnpj,
            Address = input.Address
        };

        _employers.Add(employer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<Employer>.Ok(employer);
    }

    public async Task<ServiceResult<Employer>> UpdateAsync(Guid id, UpdateEmployerInput input, CancellationToken cancellationToken)
    {
        var employer = await _employers.GetByIdAsync(id, cancellationToken);
        if (employer == null)
            return ServiceResult<Employer>.Fail(ServiceErrorType.NotFound, "Employer not found.");

        employer.Name = input.Name;
        employer.Cnpj = input.Cnpj;
        employer.Address = input.Address;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult<Employer>.Ok(employer);
    }

    public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var employer = await _employers.GetByIdAsync(id, cancellationToken);
        if (employer == null)
            return ServiceResult.Fail(ServiceErrorType.NotFound, "Employer not found.");

        _employers.Remove(employer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult.Ok();
    }
}
