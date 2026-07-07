using EcommerceMvc.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace EcommerceMvc.Repositories
{
    public class Repository<T>:IRepository<T> where T : class
    {
        private ApplicationDbContext _context;//= new();

        private DbSet<T> _db;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
           _db= _context.Set<T>();
        }

      
        //CRUD
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
          var result =  await _db.AddAsync(entity, cancellationToken);
            return result.Entity;
        }
        public void Update(T entity)
        {
            _db.Update(entity);
        }
        public void Delete(T entity)
        {
            _db.Remove(entity);
        }
        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }
        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, Object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default
            )
        {
            var entities = _db.AsQueryable();
            if (expression is not null)
                entities = entities.Where(expression);
            if (includes != null)
            {
                if (includes.Length > 1)
                {
                    foreach (var item in includes)
                    {
                        entities = entities.Include(item);
                    }
                }
                else
                    entities = entities.Include(includes[0]);
            }

            if (!tracked)
                entities = entities.AsNoTracking();
            return await entities.ToListAsync(cancellationToken);
        }
        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expression,
            Expression<Func<T, Object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default
            )
        {
            return (await GetAsync(expression, includes, tracked, cancellationToken)).FirstOrDefault();
        }
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
