﻿using BookProject.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace BookProject.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly DbSet<T> dbSet;

        public DbContext Context { get; set; }

        public BaseRepository(DbContext dbContext)
        {
            Context = dbContext;
            dbSet = dbContext.Set<T>();
        }

        public T Add(T entity)
        {
            T result = dbSet.Add(entity).Entity;
            return result;
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> entry = await dbSet.AddAsync(entity, cancellationToken);
            return entry.Entity;
        }

        public void AddRange(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken)) 
        {
            return dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public void Edit(T entity)
        {
            dbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void DeleteRange(params T[] entities)
        {
            dbSet.RemoveRange(entities);
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            var entities = dbSet.Where(where).ToList();
            dbSet.RemoveRange(entities);
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where, CancellationToken cancellationToken = default)
        {
            return await dbSet.FirstOrDefaultAsync(where, cancellationToken);
        }

        public IQueryable<T> GetAll()
        {
            return dbSet;
        }

        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query; 
        }

        public IQueryable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where);
        }

    }
}
