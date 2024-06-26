using System.Linq.Expressions;

namespace Infrastructure.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Get object by its id
        T GetByID(int? id);

        T Get(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null);

        // Same as get but async
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null);

        IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, int>>? orderBy = null, string? includes = null);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, int>>? orderBy = null, string? includes = null);

        // Add new instance of the object
        void Add(T entity);

        // Delete (remove) a single instance of an object
        void Delete(T entity);

        // Delete a range of items
        void Delete(IEnumerable<T> entities);

        // Update an item
        void Update(T entity);
    }
}
