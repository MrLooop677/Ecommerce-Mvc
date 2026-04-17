using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace EcommerceMvc.Models
{
    [PrimaryKey(nameof(ProductId), nameof(Img))]
    public class ProductSubImg
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string Img { get; set; } = string.Empty;
   
    }
}
