namespace EcommerceMvc.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; } = String.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public int Rate { get; set; }
        public string MainImage { get; set; } = String.Empty;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;
        public List<ProductSubImg> ProductSubImages { get; set; } = new();
        public List<ProductColor> ProductColors { get; set; } = new();


    }
}
