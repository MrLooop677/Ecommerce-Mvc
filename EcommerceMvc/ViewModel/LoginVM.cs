namespace EcommerceMvc.ViewModel
{
    public class LoginVM
    {
        public int Id { get; set; }
        public string userNameOREmail { get; set; }=string.Empty;
        public string Password { get; set; }=string.Empty;
        public bool RememberMe { get; set; }
    }
}
