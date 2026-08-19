using EcommerceMvc.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMvc.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {

            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            //new ApplicationUserVM()
            //{
            //    FirstName = user.FirstName,
            //    LastName = user.LastName,
            //    Email = user.Email,
            //    PhoneNumber = user.PhoneNumber,
            //    Address = user.Address
            //}

            var userVM = user.Adapt<ApplicationUserVM>();
            return View(userVM);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();

            var names = applicationUserVM.FullName.Split(' ');
            user.FirstName = names[0];
            user.LastName = names[1];
            user.PhoneNumber = applicationUserVM.PhoneNumber;
            user.Address = applicationUserVM.Address;
          var result=  await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"{error.Code}: {error.Description}");
                }

                return View(applicationUserVM);
            }
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();

            if (applicationUserVM.CurrentPassword is null || applicationUserVM.NewPassword is null)
            {
                ModelState.AddModelError(string.Empty, "Current password and new password are required.");
                return View("Index", applicationUserVM);
            }
            var result = await _userManager.ChangePasswordAsync(user, applicationUserVM.CurrentPassword, applicationUserVM.NewPassword);
            if (!result.Succeeded)
            {
                TempData["error-notification"] =
                    string.Join(", ", result.Errors.Select(e => e.Description));

                return View("Index", applicationUserVM);
            }

            TempData["SuccessMessage"] = "Password updated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
