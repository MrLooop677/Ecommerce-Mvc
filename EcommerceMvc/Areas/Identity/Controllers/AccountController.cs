using EcommerceMvc.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceMvc.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOtp> _applicationUserOtpRepository;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOtp> applicationUserOtpRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOtpRepository = applicationUserOtpRepository;
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
            var user = new ApplicationUser()
            {
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);

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
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", token, userId = user.Id }, Request.Scheme);
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

        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM
            )
        {

            if (!ModelState.IsValid)
                return View();
            var user = await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOREmail) ?? await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOREmail);
            if (user == null)
            {
                ModelState.AddModelError("", "UserName/Email not found");
                return View(resendEmailConfirmationVM);
            }

            if (user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Your Email is already confirmed, please login");
                return View(resendEmailConfirmationVM);
            }
            // send confirmation mail
            // generate token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //generate confirmation link
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", token, userId = user.Id }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email!, "Confirm your email", "Please confirm your email by clicking here: <a href='" + link + "'>Confirm Email</a>");
            return RedirectToAction("Login");
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM
            )
        {

            if (!ModelState.IsValid)
                return View();
            var user = await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOREmail) ?? await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOREmail);
            if (user == null)
            {
                ModelState.AddModelError("", "UserName/Email not found");
                return View(forgetPasswordVM);
            }

            // chck on times user send otps on day
            var userOtps = await _applicationUserOtpRepository.GetAsync(e => e.ApplicationUserId == user.Id);
            var totalOtpsSentToday = userOtps.Count(e => (DateTime.UtcNow - e.CreatedAte).TotalHours < 24);
            if (totalOtpsSentToday > 3)
            {
                ModelState.AddModelError("", "Too Many Attemps");
                return View(forgetPasswordVM);
            }

            //generate rondome otp and save in database
            var otp = new Random().Next(1000, 9999).ToString();

            await _applicationUserOtpRepository.AddAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                ApplicationUserId = user.Id,
                OTP = otp,
                CreatedAte = DateTime.UtcNow,
                IsValid = true,
                ValidTo = DateTime.UtcNow.AddDays(1)

            });
            await _applicationUserOtpRepository.CommitAsync();
            // send Otp mail
            await _emailSender.SendEmailAsync(user.Email!, "Resete Your Password", $"<h1>Use this OTP : {otp} to reset password</h1");
            return RedirectToAction("ValidateOtp", new { userId = user.Id });
        }

        public IActionResult ValidateOTP(string userId)
        {
            return View(new ValidateOTPVM { ApplicationUserId = userId });
        }
        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            var result = await _applicationUserOtpRepository.GetOneAsync(e => e.ApplicationUserId == validateOTPVM.ApplicationUserId && e.OTP == validateOTPVM.OTP && e.IsValid);
            if (result == null)
            {
                ModelState.AddModelError("", "Invalid OTP");
                return RedirectToAction(nameof(ValidateOTP), new { userId = validateOTPVM.ApplicationUserId });
            }
            return RedirectToAction("ResetPassword", new { userId = result.ApplicationUserId });
        }
        public IActionResult ResetPassword(string userId)
        {
            return View(new ResetPasswordVM { ApplicationUserId = userId });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordVM.ApplicationUserId);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(resetPasswordVM);
            }

            //generaete fake token for reset password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            var result = await _userManager.ResetPasswordAsync(user,
                   token, resetPasswordVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(resetPasswordVM);
            }

            return RedirectToAction(nameof(Login));
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
            var user = await _userManager.FindByNameAsync(loginVM.userNameOREmail) ?? await _userManager.FindByEmailAsync(loginVM.userNameOREmail);
            if (user == null)
            {
                ModelState.AddModelError("", "UserName/Email or Password is incorrect");
                return View(loginVM);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
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



        [HttpPost]

        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        [HttpGet]

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Try signing in with an external login
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl ?? "/");
            }

            // If the user cannot log in, try finding them by email
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var username = info.Principal.FindFirstValue(ClaimTypes.Name);
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Create a new user if they do not exist
                    Random random = new Random();
                    int r = random.Next(1000, 9999);
                    user = new ApplicationUser
                    {
                        UserName = username.Replace(" ", "") + r.ToString(),
                        Email = email,
                        EmailConfirmed = true
                    };
                    var createUserResult = await _userManager.CreateAsync(user);
                    if (!createUserResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error creating user.");
                        return RedirectToAction(nameof(Login));
                    }
                }

                // Ensure the external login is linked
                var existingLogins = await _userManager.GetLoginsAsync(user);
                var hasGoogleLogin = existingLogins.Any(l => l.LoginProvider == info.LoginProvider);

                if (!hasGoogleLogin)
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (!addLoginResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Error linking external login.");
                        return RedirectToAction(nameof(Login));
                    }
                }

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl ?? "/");
            }

            return RedirectToAction(nameof(Login));
        }

    }
}
