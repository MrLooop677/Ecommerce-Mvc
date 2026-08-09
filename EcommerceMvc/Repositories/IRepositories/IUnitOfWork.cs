using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using System.Data;

namespace EcommerceMvc.Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository ProductRepository { get; }
        IRepository<Category> CategoryRepository { get; }
        IRepository<Brand> BrandRepository { get; }
        IRepository<ProductColor> ProductColorRepository { get; }
        IRepository<ProductSubImg> ProductSubImgRepository { get; }
        Task CommitAsync(CancellationToken cancellationToken = default);
        SqlServerTransaction BeginTransaction();
    }
}
