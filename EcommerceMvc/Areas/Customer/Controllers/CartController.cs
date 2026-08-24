using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceMvc.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;

        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AddToCart(int productId,int count,CancellationToken cancellationToken) {
            var user = await _userManager.GetUserAsync(User);
            if(user is null)
                return NotFound();


            var productInDB = await _cartRepository.GetOneAsync(e=>e.ProductId==productId && e.ApplicationUserId==user.Id);

            if(productInDB != null)
            {
                productInDB.count += count;
                await _cartRepository.CommitAsync(cancellationToken);
                TempData["success"] = "Product added to cart successfully!";
                return RedirectToAction("Index", "Home");
            }
           await _cartRepository.AddAsync(new() {

               ProductId = productId,
               ApplicationUserId = user.Id,
               count = count,

           },cancellationToken);
            await _cartRepository.CommitAsync(cancellationToken);
            TempData["success"] = "Product added to cart successfully!";
            return RedirectToAction("Index","Home");
        
        }
    }
}
