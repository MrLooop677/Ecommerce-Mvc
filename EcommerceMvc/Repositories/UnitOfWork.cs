using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using System.Data;

namespace EcommerceMvc.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(
            IProductRepository productRepository, 
            IRepository<Category> categoryRepository,
            IRepository<Brand> brandRepository,
            IRepository<ProductColor> productColorRepository,
            IRepository<ProductSubImg> productSubImgRepository,
            ApplicationDbContext context
            )
        {
            ProductRepository = productRepository;
            CategoryRepository = categoryRepository;
            BrandRepository = brandRepository;
            ProductColorRepository = productColorRepository;
            ProductSubImgRepository = productSubImgRepository;
            _context = context;
        }
        public void Dispose()
        {
            _context.Dispose();
        }
        public ApplicationDbContext _context { get; }

        public IProductRepository ProductRepository {get;}

        public IRepository<Category> CategoryRepository {get;}

        public IRepository<Brand> BrandRepository {get;}

        public IRepository<ProductColor> ProductColorRepository { get; }
        public IRepository<ProductSubImg> ProductSubImgRepository {get;}

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

        public SqlServerTransaction BeginTransaction() => (SqlServerTransaction)_context.Database.BeginTransaction();
    }
}
