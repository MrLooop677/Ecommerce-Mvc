namespace EcommerceMvc.Repositories.IRepositories
{
    public interface IProductColorRepository: IRepository<ProductColor>
    {
         void RemoveRange(IEnumerable<ProductColor> productColors);
       
    }
}
