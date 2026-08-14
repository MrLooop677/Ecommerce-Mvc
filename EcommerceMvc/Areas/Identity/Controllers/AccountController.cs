using EcommerceMvc.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMvc.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
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
            var user = new ApplicationUser() {
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName
            };
            var result = await _userManager.CreateAsync(user,registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Code);
                }
                return View(registerVM);

            }

            // send confirmation mail

            // generate token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //generate confirmation link
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area="Identity", token ,userId=user.Id},Request.Scheme);
            await _emailSender.SendEmailAsync(registerVM.Email, "Confirm your email", "Please confirm your email by clicking here: <a href='" + link + "'>Confirm Email</a>");

            return RedirectToAction("Login");

        }
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                TempData["error-notification"] = "Invalid User";

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                 TempData["error-notification"] = "Invalid OR Expired Token";
            else
               TempData["success-notification"] = "Email confirmed successfully";

            return RedirectToAction("Login", "Account", new { area = "Identity" });

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
                if (result.IsLockedOut)
                    ModelState.AddModelError("", "Too many attemps Your account is locked out, please try again later");
                else if (!user.EmailConfirmed)
                    ModelState.AddModelError("", "Please Confirm Your Email First!!");
                else
                    ModelState.AddModelError("", "UserName/Email or Password is incorrect");
                return View(loginVM);
            }
            return RedirectToAction("Index", "Home", new { area = "Customer" });

        } 

    }
}
