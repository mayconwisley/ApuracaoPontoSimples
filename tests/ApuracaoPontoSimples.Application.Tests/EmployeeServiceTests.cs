using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using ApuracaoPontoSimples.Application.UseCases.Employees;
using ApuracaoPontoSimples.Domain.Entities;
using System.Collections.Concurrent;
using Xunit;

namespace ApuracaoPontoSimples.Application.Tests;

public sealed class EmployeeServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenEmployerMissing_ReturnsValidationError()
    {
        var employees = new FakeEmployeeRepository();
        var employers = new FakeEmployerRepository(exists: false);
        var unitOfWork = new FakeUnitOfWork();

        var service = new EmployeeService(employees, employers, unitOfWork);

        var input = new CreateEmployeeInput(
            "Maria",
            "123",
            new DateOnly(2024, 1, 1),
            Guid.NewGuid(),
            new ScheduleConfigInput(
                TimeSpan.FromHours(8),
                null,
                null,
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(5),
                null,
                null,
                null,
                true,
                true));

        var result = await service.CreateAsync(input, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public async Task CreateAsync_WhenScheduleProvided_AssignsEmployeeId()
    {
        var employees = new FakeEmployeeRepository();
        var employers = new FakeEmployerRepository(exists: true);
        var unitOfWork = new FakeUnitOfWork();

        var service = new EmployeeService(employees, employers, unitOfWork);

        var input = new CreateEmployeeInput(
            "Joao",
            "321",
            new DateOnly(2024, 2, 1),
            Guid.NewGuid(),
            new ScheduleConfigInput(
                TimeSpan.FromHours(8),
                null,
                null,
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(5),
                null,
                null,
                null,
                true,
                true));

        var result = await service.CreateAsync(input, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value!.Schedule);
        Assert.Equal(result.Value.Id, result.Value.Schedule!.EmployeeId);
    }

    private sealed class FakeEmployeeRepository : IEmployeeRepository
    {
        private readonly ConcurrentDictionary<Guid, Employee> _store = new();

        public Task<IReadOnlyList<Employee>> GetAllWithEmployerAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Employee>>(_store.Values.ToList());

        public Task<Employee?> GetByIdWithScheduleAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult(_store.TryGetValue(id, out var employee) ? employee : null);

        public Task<Employee?> GetByIdWithScheduleReadOnlyAsync(Guid id, CancellationToken cancellationToken)
            => GetByIdWithScheduleAsync(id, cancellationToken);

        public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => GetByIdWithScheduleAsync(id, cancellationToken);

        public void Add(Employee employee) => _store[employee.Id] = employee;

        public void Remove(Employee employee) => _store.TryRemove(employee.Id, out _);
    }

    private sealed class FakeEmployerRepository : IEmployerRepository
    {
        private readonly bool _exists;

        public FakeEmployerRepository(bool exists)
        {
            _exists = exists;
        }

        public Task<IReadOnlyList<Employer>> GetAllAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Employer>>(Array.Empty<Employer>());

        public Task<Employer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult<Employer?>(null);

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult(_exists);

        public void Add(Employer employer) { }

        public void Remove(Employer employer) { }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<IAppTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
            => Task.FromResult<IAppTransaction>(new FakeTransaction());

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => Task.FromResult(1);
    }

    private sealed class FakeTransaction : IAppTransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task RollbackAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
