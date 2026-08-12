using EcommerceMvc.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMvc.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _userManager.CreateAsync(new()
            {
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName
            }, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Code);
                }
                return View(registerVM);

            }
            return RedirectToAction("Login");

        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View();
            var user =await _userManager.FindByNameAsync(loginVM.userNameOREmail) ??await _userManager.FindByEmailAsync(loginVM.userNameOREmail);
            if (user == null)
            {
                ModelState.AddModelError("", "UserName/Email or Password is incorrect");
                return View(loginVM);
            }
            var result = await _signInManager.PasswordSignInAsync(user,loginVM.Password,loginVM.RememberMe,lockoutOnFailure:true);
            if (!result.Succeeded) { 
                if(result.IsLockedOut)
                    ModelState.AddModelError("", "Too many attemps Your account is locked out, please try again later");
                else
                    ModelState.AddModelError("", "UserName/Email or Password is incorrect");
                return View(loginVM);
            }
            return RedirectToAction("Index", "Home", new { area = "Customer" });

        } 

    }
}
