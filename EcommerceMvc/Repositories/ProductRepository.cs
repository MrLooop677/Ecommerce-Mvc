using EcommerceMvc.Repositories.IRepositories;

namespace EcommerceMvc.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _context;//= new();

        public ProductRepository(ApplicationDbContext context): base(context) 
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken)
        {

            await _context.Products.AddRangeAsync(products, cancellationToken);

        }
    }
}
