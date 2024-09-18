using BookProject.Interface;
using BookProject.Model;
using BookProject.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookProject
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookProjectDbContext _dbContext;
        public IBookRepository BookRepository { get; }

        public UnitOfWork(BookProjectDbContext dbContext)
        {
            this._dbContext = dbContext;

            BookRepository = new BookRepository(dbContext);
        }


        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public IDbContextTransaction StartNewTransaction()
        {
            return _dbContext.Database.BeginTransaction();
        }

        public Task<IDbContextTransaction> StartNewTransactionAsync()
        {
            return _dbContext.Database.BeginTransactionAsync();
        }

        public Task<int> ExecuteSqlCommandAsync(string sql, object[] parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _dbContext.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected virtual method to allow derived classes to override
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                _dbContext?.Dispose();
            }
        }
    }
}
