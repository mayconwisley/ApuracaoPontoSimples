using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using ApuracaoPontoSimples.Application.UseCases.TimeCards;
using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Domain.Enums;
using Xunit;

namespace ApuracaoPontoSimples.Application.Tests;

public sealed class TimeCardServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenDayOutsideRange_ReturnsValidationError()
    {
        var employees = new FakeEmployeeRepository();
        var timeCards = new FakeTimeCardRepository();
        var unitOfWork = new FakeUnitOfWork();
        var calculator = new FakeTimeCardCalculator();

        var service = new TimeCardService(employees, timeCards, unitOfWork, calculator);

        var input = new CreateTimeCardInput(
            employees.EmployeeId,
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 10),
            new List<DayEntryInput>
            {
                new(
                    new DateOnly(2024, 1, 11),
                    DayCode.None,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    HolidayType.None,
                    false,
                    false,
                    null,
                    AbsenceType.None)
            });

        var result = await service.CreateAsync(input, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public async Task GetAsync_WhenNoTimeCard_ReturnsNotFound()
    {
        var employees = new FakeEmployeeRepository();
        var timeCards = new FakeTimeCardRepository();
        var unitOfWork = new FakeUnitOfWork();
        var calculator = new FakeTimeCardCalculator();

        var service = new TimeCardService(employees, timeCards, unitOfWork, calculator);

        var result = await service.GetAsync(
            employees.EmployeeId,
            new DateOnly(2024, 2, 1),
            new DateOnly(2024, 2, 5),
            CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    private sealed class FakeEmployeeRepository : IEmployeeRepository
    {
        public Guid EmployeeId { get; } = Guid.NewGuid();

        public Task<IReadOnlyList<Employee>> GetAllWithEmployerAsync(CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Employee>>(Array.Empty<Employee>());

        public Task<Employee?> GetByIdWithScheduleAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult<Employee?>(CreateEmployee());

        public Task<Employee?> GetByIdWithScheduleReadOnlyAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult<Employee?>(CreateEmployee());

        public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => Task.FromResult<Employee?>(CreateEmployee());

        public void Add(Employee employee) { }

        public void Remove(Employee employee) { }

        private Employee CreateEmployee()
            => new()
            {
                Id = EmployeeId,
                Name = "Test",
                EmployerId = Guid.NewGuid(),
                Schedule = new ScheduleConfig
                {
                    DailyHours = TimeSpan.FromHours(8),
                    UseDailyHoursAsY13 = true,
                    SaturdayCountsAsBank = true
                }
            };
    }

    private sealed class FakeTimeCardRepository : ITimeCardRepository
    {
        public Task<IReadOnlyList<TimeCard>> GetOverlappingAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<TimeCard>>(Array.Empty<TimeCard>());

        public Task AddAsync(TimeCard timeCard, CancellationToken cancellationToken) => Task.CompletedTask;

        public void RemoveRange(IEnumerable<TimeCard> timeCards) { }
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

    private sealed class FakeTimeCardCalculator : ITimeCardCalculator
    {
        public TimeCard Calculate(TimeCard timeCard, ScheduleConfig schedule) => timeCard;
    }
}
