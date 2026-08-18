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
          var user= await _userManager.GetUserAsync(User);

            if(user == null) 
                return NotFound();

            //new ApplicationUserVM()
            //{
            //    FirstName = user.FirstName,
            //    LastName = user.LastName,
            //    Email = user.Email,
            //    PhoneNumber = user.PhoneNumber,
            //    Address = user.Address
            //}
              
            var userVM=user.Adapt<ApplicationUserVM>();
            return View(userVM);
        }
    }
}
