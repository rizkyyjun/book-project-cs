using BookProject.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookProject
{
    public interface IUnitOfWork : IDisposable
    {
        public IBookRepository BookRepository { get; }

        void Save();
        Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken));
        IDbContextTransaction StartNewTransaction();
        Task<IDbContextTransaction> StartNewTransactionAsync();
        Task<int> ExecuteSqlCommandAsync(string sql, object[] parameters, CancellationToken cancellationToken = default(CancellationToken));

    }
}
