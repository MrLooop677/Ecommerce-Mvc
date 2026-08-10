using Microsoft.AspNetCore.Mvc;

namespace EcommerceMvc.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
    }
}
