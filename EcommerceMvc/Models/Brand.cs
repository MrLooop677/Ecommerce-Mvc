using EcommerceMvc.Validations;

namespace EcommerceMvc.Models
{
    public class Brand
    {
        public int ID { get; set; }
        [CustomeLengthAttribute(3, 100)]
        public string Name { get; set; } = String.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
        public List<Product> products { get; set; }
        public string Img { get; set; } = "defaultImg.png";

    }
}
