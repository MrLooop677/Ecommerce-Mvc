using System.ComponentModel.DataAnnotations;

namespace EcommerceMvc.ViewModel
{
    public class UpdateBrandVM
    {
        public int ID { get; set; }

        [MinLength(3, ErrorMessage = "Brand name must be at least 3 characters long")]
        [MaxLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
        public string Name { get; set; } = String.Empty;
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]

        public string? Description { get; set; }
        [Required(ErrorMessage = "Status is required")]

        public bool Status { get; set; }

        public string? Img { get; set; }

        public IFormFile? NewImg { get; set; }

    } 
}
