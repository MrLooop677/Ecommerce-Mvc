namespace EcommerceMvc.ViewModel
{
    public class ProductWithRelatedVM
    {
        public Product product { get; set; } = default!;
        public List<Product> relatedProducts { get; set; } = [];


    }
}
