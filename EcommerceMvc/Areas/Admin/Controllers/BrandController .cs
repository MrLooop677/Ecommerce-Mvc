using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult Edit (Brand brand)
        {
            _context.Brands.Update(brand); 
            _context.SaveChanges();  
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
            return RedirectToAction(nameof(Index));
        }
    }
}
