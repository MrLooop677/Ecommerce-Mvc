using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMvc.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CategoryController : Controller
    {
        ApplicationDbContext _context = new ();
        public IActionResult Index()
        {
            var Catagories = _context.Categories.AsQueryable().AsNoTracking();
            return View(Catagories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create ()
        {
       
            return View(new Category());
        }
        [HttpPost]
        public IActionResult Create (Category category)
        {
            if (!ModelState.IsValid)
            {

                ModelState.AddModelError(string.Empty, "Please fill all required fields correctly.");

            return View(category);
            }
            _context.Categories.Add(category);
            _context.SaveChanges();  
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit (int id)
        {
            var selectedCategory = _context.Categories.FirstOrDefault(c=>c.ID==id);
            if (selectedCategory == null) {
                return RedirectToAction("NotFoundPage", "Home");
            }
       
            return View(selectedCategory);
        }
        [HttpPost]
        public IActionResult Edit (Category category)
        {
            if (!ModelState.IsValid)
            {

                ModelState.AddModelError(string.Empty, "");
                return View(category);
            }
            _context.Categories.Update(category); 
            _context.SaveChanges();  
            return RedirectToAction(nameof(Index));
        }
        //[HttpGet]
        //public IActionResult Delete ()
        //{
            
        //    return View();
        //}
        public IActionResult Delete (int id)
        {
            var deletedItem=_context.Categories.FirstOrDefault(c => c.ID == id);
            if (deletedItem == null) {
                return RedirectToAction("NotFoundPage", "Home");
            }
           

            _context.Categories.Remove(deletedItem);
            
            _context.SaveChanges();  
            return RedirectToAction(nameof(Index));
        }
    }
}
