﻿using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public virtual T Get(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null)
        {
            if (includes == null)   // not joining other objects
            {
                if (!trackChanges)
                {
                    return _context.Set<T>().Where(predicate).AsNoTracking().FirstOrDefault();
                }
                else
                {
                    return _context.Set<T>().Where(predicate).FirstOrDefault();
                }
            }
            else
            {
                IQueryable<T> queryable = _context.Set<T>();
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }

                if (!trackChanges)
                {
                    return queryable.Where(predicate).AsNoTracking().FirstOrDefault();
                }
                else
                {
                    return queryable.Where(predicate).FirstOrDefault();
                }
            }
        }

        public virtual IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, int>>? orderBy = null, string? includes = null)
        {
            IQueryable<T> queryable = _context.Set<T>();
            if (predicate != null && includes == null)
            {
                return _context.Set<T>().Where(predicate).AsEnumerable();
            }
            else if (includes != null)
            {
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }
            }

            if (predicate == null)
            {
                if (orderBy == null)
                {
                    return queryable.AsEnumerable();
                }
                else
                {
                    return queryable.OrderBy(orderBy).ToList();
                }
            }
            else
            {
                if (orderBy == null)
                {
                    return queryable.Where(predicate).ToList();
                }
                else
                {
                    return queryable.Where(predicate).OrderBy(orderBy).ToList();
                }
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, int>>? orderBy = null, string? includes = null)
        {
            IQueryable<T> queryable = _context.Set<T>();
            if (predicate != null && includes == null)
            {
                return _context.Set<T>().Where(predicate).AsEnumerable();
            }
            else if (includes != null)
            {
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }
            }

            if (predicate == null)
            {
                if (orderBy == null)
                {
                    return queryable.AsEnumerable();
                }
                else
                {
                    return await queryable.OrderBy(orderBy).ToListAsync();
                }
            }
            else
            {
                if (orderBy == null)
                {
                    return await queryable.Where(predicate).ToListAsync();
                }
                else
                {
                    return await queryable.Where(predicate).OrderBy(orderBy).ToListAsync();
                }
            }
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null)
        {
            if (includes == null)   // not joining other objects
            {
                if (!trackChanges)
                {
                    return await _context.Set<T>().Where(predicate).AsNoTracking().FirstOrDefaultAsync();
                }
                else
                {
                    return await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();
                }
            }
            else
            {
                IQueryable<T> queryable = _context.Set<T>();
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty);
                }

                if (!trackChanges)
                {
                    return queryable.Where(predicate).AsNoTracking().FirstOrDefault();
                }
                else
                {
                    return queryable.Where(predicate).FirstOrDefault();
                }
            }
        }

        public virtual T GetByID(int? id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
