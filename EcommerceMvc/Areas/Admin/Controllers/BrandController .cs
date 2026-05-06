using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace EcommerceMvc.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class BrandController : Controller
    {
        ApplicationDbContext _context = new ();
        public IActionResult Index()
        {
            var Brands = _context.Brands.AsQueryable().AsNoTracking();
            return View(Brands.Select(e => new { 
                e.ID,
                e.Name,
                e.Description,
                e.Status,
                e.Img

            }).AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create ()
        {
       
            return View();
        }
        [HttpPost]
        public IActionResult Create (Brand brand, IFormFile image)
        {
            if (image is not null && image.Length > 0) {
                //create and set img
                var fileName = Guid.NewGuid().ToString() +Path.GetExtension(image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images",fileName);
                using (var stream = System.IO.File.Create(filePath))
                { 
                    image.CopyTo(stream);           
                }
                brand.Img = fileName;
            }
            _context.Brands.Add(brand);
            _context.SaveChanges();
            //Response.Cookies.Append("success-notification", "Brand created successfully!");
            TempData["success-notification"] = "Brand created successfully!";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit (int id)
        {
            var selectedbrand = _context.Brands.FirstOrDefault(c=>c.ID==id);
            if (selectedbrand == null) {
                return RedirectToAction("NotFoundPage", "Home");
            }
       
            return View(selectedbrand);
        }
        [HttpPost]
        public IActionResult Edit (Brand brand,IFormFile? image, int id)
        {
            var selectedbrand = _context.Brands.AsNoTracking().FirstOrDefault(c => c.ID == brand.ID);
            if (selectedbrand is null)
                return RedirectToAction("NotFoundPage", "Home");

            if (image is not null) {
                if (image is not null && image.Length > 0)
                {
                    //create and set img
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        image.CopyTo(stream);
                    }

                    //Remove old img from wwwroot
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", selectedbrand.Img);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);

                    //Save New Img
                    brand.Img = fileName;
                }
            }
            else
            {
                brand.Img = selectedbrand.Img;
            }
            _context.Brands.Update(brand); 
            _context.SaveChanges();
            TempData["success-notification"] = "Brand updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        //[HttpGet]
       
        public IActionResult Delete (int id)
        {
            var deletedItem=_context.Brands.FirstOrDefault(c => c.ID == id);
            if (deletedItem == null) {
                return RedirectToAction("NotFoundPage", "Home");
            }
           

            _context.Brands.Remove(deletedItem);
            
            _context.SaveChanges();
            TempData["success-notification"] = "Brand deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
