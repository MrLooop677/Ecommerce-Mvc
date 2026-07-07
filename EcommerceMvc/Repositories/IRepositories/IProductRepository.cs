using Microsoft.EntityFrameworkCore;

namespace EcommerceMvc.Repositories.IRepositories
{
    public interface IProductRepository:IRepository<Product>
    {
        Task AddRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken);
      
    }
}
