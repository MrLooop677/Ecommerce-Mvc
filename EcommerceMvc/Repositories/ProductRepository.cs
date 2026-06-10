namespace EcommerceMvc.Repositories
{
    public class ProductRepository : Repository<Product>
    {
        private ApplicationDbContext _context = new();

        public async Task AddRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken)
        {

            await _context.Products.AddRangeAsync(products, cancellationToken);

        }
    }
}
