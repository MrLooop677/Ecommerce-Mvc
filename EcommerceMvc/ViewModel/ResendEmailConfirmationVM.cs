using System.ComponentModel.DataAnnotations;

namespace EcommerceMvc.ViewModel
{
    public class ResendEmailConfirmationVM
    {
        public int Id { get; set; }
        [Required]
        public string UserNameOREmail { get; set; } = string.Empty;
    }
}
