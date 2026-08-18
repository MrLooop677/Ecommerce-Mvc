using EcommerceMvc.Utitlies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =$"{SataticData.SUPER_ADMIN_ROLE}, {SataticData.ADMIN_ROLE}, {SataticData.EMPLOYEE_ROLE}")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}
