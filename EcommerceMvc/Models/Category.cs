using System.ComponentModel.DataAnnotations;

namespace EcommerceMvc.Models
{
    public class Category
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Category name is required")]
        [MinLength(3, ErrorMessage = "Category name must be at least 3 characters long")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string Name { get; set; } = String.Empty;
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Status is required")]
        public bool Status { get; set; }
        public List<Product> products { get; set; }= new();
    }
}
