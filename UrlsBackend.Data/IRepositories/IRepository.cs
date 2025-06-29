﻿using System.Linq.Expressions;


public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    IQueryable<T> GetAll();
    Task<T> FindAsync(Expression<Func<T, bool>> predicate);
}
