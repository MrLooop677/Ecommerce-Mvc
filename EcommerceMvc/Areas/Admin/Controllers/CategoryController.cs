using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMvc.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CategoryController : Controller
    {
        //ApplicationDbContext _context = new ();
        //Repository<Category> _categoryRepository = new();
        private readonly IRepository<Category> _categoryRepository;//=new Repository<Category>();
        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var Catagories = await _categoryRepository.GetAsync(tracked:false,cancellationToken: cancellationToken);
            return View(Catagories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View(new Category());
        }
        [HttpPost] 
        public async Task<IActionResult> Create(Category category, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {

                ModelState.AddModelError(string.Empty, "Please fill all required fields correctly.");

                return View(category);
            }

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id,CancellationToken cancellationToken)
        {
            var selectedCategory =await _categoryRepository.GetOneAsync(c => c.ID == id,cancellationToken: cancellationToken);
            if (selectedCategory == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
              
            return View(selectedCategory);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Category category,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {

                ModelState.AddModelError(string.Empty, "");
                return View(category);
            }
            _categoryRepository.Update(category);
           await _categoryRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id,CancellationToken cancellationToken) 
        {
            var deletedItem =await _categoryRepository.GetOneAsync(c => c.ID == id,cancellationToken: cancellationToken);
            if (deletedItem == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }


            _categoryRepository.Delete(deletedItem);
            await _categoryRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
