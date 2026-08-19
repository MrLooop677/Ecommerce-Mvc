using EcommerceMvc.Utitlies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SataticData.SUPER_ADMIN_ROLE}")]
    public class UserController : Controller
    {
        private UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {


            return View(_userManager.Users);
        }
        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            if(_userManager.IsInRoleAsync(user, SataticData.SUPER_ADMIN_ROLE).GetAwaiter().GetResult())
            {
                TempData["error-notification"] = $"You can not lock/unlock {user.FirstName} {user.LastName}";
                return RedirectToAction(nameof(Index));
            }
            user.LockoutEnabled = !user.LockoutEnabled;
            if (user.LockoutEnabled)
                   user.LockoutEnd = DateTime.Now.AddDays(30);
            else
                user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);
            TempData["success-notification"] = $"Update Status {user.FirstName} {user.LastName}";

            return RedirectToAction(nameof(Index));

        }
    }
}
