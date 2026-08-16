using System.ComponentModel.DataAnnotations;

namespace EcommerceMvc.ViewModel
{
    public class ResetPasswordVM
    {
        public int Id { get; set; }
        [Required]
        public string Password { get; set; }=string.Empty;
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }=string.Empty;
        public string ApplicationUserId { get; set; }= string.Empty;
    }
}
