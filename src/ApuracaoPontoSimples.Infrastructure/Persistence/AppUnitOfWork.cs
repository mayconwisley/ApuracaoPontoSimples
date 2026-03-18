using ApuracaoPontoSimples.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApuracaoPontoSimples.Infrastructure.Persistence;

public sealed class AppUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public AppUnitOfWork(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IAppTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        return new AppTransaction(transaction);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => _db.SaveChangesAsync(cancellationToken);

    private sealed class AppTransaction : IAppTransaction
    {
        private readonly IDbContextTransaction _transaction;

        public AppTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync(CancellationToken cancellationToken)
            => _transaction.CommitAsync(cancellationToken);

        public Task RollbackAsync(CancellationToken cancellationToken)
            => _transaction.RollbackAsync(cancellationToken);

        public ValueTask DisposeAsync() => _transaction.DisposeAsync();
    }
}
