using EcommerceMvc.Utitlies;
using EcommerceMvc.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace EcommerceMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SataticData.SUPER_ADMIN_ROLE}, {SataticData.ADMIN_ROLE}, {SataticData.EMPLOYEE_ROLE}")]
    public class PromotionController : Controller
    {
       
       private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Product> _productRepository;
        public PromotionController(IUnitOfWork unitOfWork,IRepository<Product> productRepository)
        {
            this._unitOfWork = unitOfWork;
            this._productRepository = productRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var Promotions =await _unitOfWork.PromotionRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            return View(Promotions);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.products = await _productRepository.GetAsync(tracked:false); 

            return View(new Promotion());
        }
        [HttpPost] 
        public async Task<IActionResult> Create(Promotion promotion,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(promotion);

            await _unitOfWork.PromotionRepository.AddAsync(promotion, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            //Response.Cookies.Append("success-notification", "Promotion created successfully!");
            TempData["success-notification"] = "Promotion created successfully!";
            return RedirectToAction("Index", "Cart", new { area = "Customer" });
        }
        [HttpGet]
        [Authorize(Roles = $"{SataticData.SUPER_ADMIN_ROLE},{SataticData.ADMIN_ROLE}")]

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            //var selectedbrand = _context.Brands.FirstOrDefault(c => c.ID == id);
            var selectedpromotion = await _unitOfWork.PromotionRepository.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);

            if (selectedpromotion == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

           
            return View(selectedpromotion);

        }
        [HttpPost]
        [Authorize(Roles = $"{SataticData.SUPER_ADMIN_ROLE},{SataticData.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Promotion updatePromotionVM,CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid) 
                return View(updatePromotionVM);
            //var selectedbrand = _context.Brands.AsNoTracking().FirstOrDefault(c => c.ID == updateBrandVM.Id);
            var selectedpromotion =await _unitOfWork.PromotionRepository.GetOneAsync(c => c.Id == updatePromotionVM.Id,tracked:false, cancellationToken: cancellationToken);
            if (selectedpromotion is null)
                return RedirectToAction("NotFoundPage", "Home");
          
            _unitOfWork.PromotionRepository.Update(selectedpromotion);
            await _unitOfWork.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Promotion updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        //[HttpGet]
        [Authorize(Roles = $"{SataticData.SUPER_ADMIN_ROLE},{SataticData.ADMIN_ROLE}")]

        public async Task<IActionResult> Delete(int id)
        {
            //var deletedItem = _context.Brands.FirstOrDefault(c => c.ID == id);
            var deletedItem = await _unitOfWork.PromotionRepository.GetOneAsync(c => c.Id == id);
            if (deletedItem == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }


            //_context.Brands.Remove(deletedItem);
            _unitOfWork.PromotionRepository.Delete(deletedItem);

            //_context.SaveChanges();
            await _unitOfWork.CommitAsync();
            TempData["success-notification"] = "Promotion deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
