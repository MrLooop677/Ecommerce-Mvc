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
        public async Task<IActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Product, e => e.ApplicationUser]);

            return View(cart);
        }
        public async Task<IActionResult> AddToCart(int productId, int count, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();


            var productInDB = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if (productInDB != null)
            {
                productInDB.count += count;
                await _cartRepository.CommitAsync(cancellationToken);
                TempData["success"] = "Product added to cart successfully!";
                return RedirectToAction("Index", "Home");
            }
            await _cartRepository.AddAsync(new()
            {

                ProductId = productId,
                ApplicationUserId = user.Id,
                count = count,

            }, cancellationToken);
            await _cartRepository.CommitAsync(cancellationToken);
            TempData["success"] = "Product added to cart successfully!";
            return RedirectToAction("Index", "Home");

        }


        public async Task<IActionResult> IncreamentProduct(int productId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();
            var product = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);
            if (product == null) return NotFound();

            product.count++;
            await _cartRepository.CommitAsync(cancellationToken);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DecreamentProduct(int productId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();
            var product = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);
            if (product == null) return NotFound();


            if (product.count <= 1)
                _cartRepository.Delete(product);

            else
                product.count--;

            await _cartRepository.CommitAsync(cancellationToken);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteProduct(int productId, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();
            var product = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);
            if (product == null) return NotFound();
            _cartRepository.Delete(product);
            await _cartRepository.CommitAsync(cancellationToken);
            return RedirectToAction("Index");
        }
    }
}
