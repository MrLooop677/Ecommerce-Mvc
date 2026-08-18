using EcommerceMvc.Utitlies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SataticData.SUPER_ADMIN_ROLE}, {SataticData.ADMIN_ROLE}, {SataticData.EMPLOYEE_ROLE}")]
    public class CategoryController : Controller
    {
        //ApplicationDbContext _context = new ();
        //Repository<Category> _categoryRepository = new();
        //private readonly IRepository<Category> _categoryRepository;//=new Repository<Category>();
        private readonly  IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var Catagories = await _unitOfWork.CategoryRepository.GetAsync(tracked:false,cancellationToken: cancellationToken);
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

            await _unitOfWork.CategoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = $"{SataticData.SUPER_ADMIN_ROLE},{SataticData.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id,CancellationToken cancellationToken)
        {
            var selectedCategory =await _unitOfWork.CategoryRepository.GetOneAsync(c => c.ID == id,cancellationToken: cancellationToken);
            if (selectedCategory == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
              
            return View(selectedCategory);
        }
        [HttpPost]
        [Authorize(Roles = $"{SataticData.SUPER_ADMIN_ROLE},{SataticData.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Category category,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {

                ModelState.AddModelError(string.Empty, "");
                return View(category);
            }
            _unitOfWork.CategoryRepository.Update(category);
           await _unitOfWork.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = $"{SataticData.SUPER_ADMIN_ROLE},{SataticData.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id,CancellationToken cancellationToken) 
        {
            var deletedItem =await _unitOfWork.CategoryRepository.GetOneAsync(c => c.ID == id,cancellationToken: cancellationToken);
            if (deletedItem == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }


            _unitOfWork.CategoryRepository.Delete(deletedItem);
            await _unitOfWork.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
