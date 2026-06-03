using EcommerceMvc.Validations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMvc.Models
{
    public class Brand
    {
        public int ID { get; set; }

        public string Name { get; set; } = String.Empty;

        public string? Description { get; set; }

        public bool Status { get; set; }

        public string Img { get; set; } = "defaultImg.png";
        public List<Product> products { get; set; } = new List<Product>();

    }
}
