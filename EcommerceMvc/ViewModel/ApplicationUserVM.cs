namespace EcommerceMvc.ViewModel
{
    public class ApplicationUserVM
    {
        public string FullName { get; set; }=string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; }=string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
