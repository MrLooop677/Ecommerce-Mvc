namespace EcommerceMvc.Models
{
    public class Brand
    {
        public int ID { get; set; }
        public string Name { get; set; } = String.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
        public List<Product> products { get; set; }

    }
}
