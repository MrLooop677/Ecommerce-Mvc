using EcommerceMvc.Models;
using EcommerceMvc.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcommerceMvc.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

     


        public IActionResult Index(FilterProductVM FilterProductVM,int page=1)
        {
            const decimal discount = 50;
            const double pageSize= 4.0;
            var products = _context.Products.Include(p => p.Category).AsNoTracking().AsQueryable();
            if (FilterProductVM.name != null)
            {

                products = products.Where(p => p.Name.Contains(FilterProductVM.name));
                ViewBag.Name = FilterProductVM.name;
            }
             if (FilterProductVM.minPrice !=null) {
                products = products.Where(p => p.Price - (p.Price * p.Discount/ 100)>FilterProductVM.minPrice);
                ViewBag.MinPrice = FilterProductVM.minPrice;

            }
             if (FilterProductVM.maxPrice !=null) {
                products = products.Where(p => p.Price - (p.Price * p.Discount/ 100)<FilterProductVM.maxPrice);
                ViewBag.MaxPrice = FilterProductVM.maxPrice;

            }
            if (FilterProductVM.categoryId != null) {
                products = products.Where(p => p.CategoryId == FilterProductVM.categoryId);
                ViewBag.CategoryId = FilterProductVM.categoryId;
            }
            if (FilterProductVM.brandId != null) {
                products = products.Where(p => p.BrandId == FilterProductVM.brandId);
                ViewBag.BrandId = FilterProductVM.brandId;
            }
            if (FilterProductVM.isHot) {
                products = products.Where(p => p.Discount> discount);
                ViewBag.IsHot = FilterProductVM.isHot;
            }

            var categories = _context.Categories.AsNoTracking();
            var brands = _context.Brands.AsNoTracking();
            ViewData["Categories"] = categories.AsEnumerable();
            ViewBag.Brands = brands.AsEnumerable();

            //pagination
            ViewBag.totalPage = Math.Ceiling(products.Count() / pageSize);
            ViewBag.currentPage = page;
            products =products.Skip((page - 1) * (int)pageSize).Take((int)pageSize);
             
            return View(products.AsEnumerable());
        }

        public IActionResult Privacy()
        {
            
            return View();
        }


        // product details
        public async Task<IActionResult> Item(int id,CancellationToken cancellationToken)
        {   
            var product =await _context.Products.Include(p=>p.Category).AsNoTracking().FirstOrDefaultAsync(p=>p.ID==id,cancellationToken);

           
            if (product == null)
            {
                return NotFound();
            }
             var relatedProduct = await _context.Products.Include(p => p.Category).AsNoTracking().Where(p => p.CategoryId == product.CategoryId && p.ID != id).ToListAsync(cancellationToken);
            
            return View(new ProductWithRelatedVM { 
                product = product,
                relatedProducts = relatedProduct
            
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
