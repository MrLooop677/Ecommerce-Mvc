using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace EcommerceMvc.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IRepository<Product> _productRepository;

        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Promotion> promotionRepository, IRepository<Product> productRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _promotionRepository = promotionRepository;
            _productRepository = productRepository;
        }
        //public async Task<IActionResult> Index(string code, CancellationToken cancellationToken)
        //{

        //    var user = await _userManager.GetUserAsync(User);
        //    if (user is null)
        //        return NotFound();
        //    var promotion = await _promotionRepository.GetOneAsync(e => e.Code == code && e.IsValid);
        //    var cart = await _cartRepository.GetAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Product, e => e.ApplicationUser]);

        //    if (promotion is not null) { 

        //      var result= cart.FirstOrDefault(e=>e.ProductId==promotion.ProductId);
        //        if(result is not null)
        //            result.Price -= (result.Product.Price * promotion.Discount / 100);
        //        await _cartRepository.CommitAsync(cancellationToken);
        //    }
        //    return View(cart);
        //}
        [HttpGet]
        public async Task<IActionResult> Index(
    CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return Challenge();

            var cart = await _cartRepository.GetAsync(
                e => e.ApplicationUserId == user.Id,
                includes: [e => e.Product]);

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyPromotion(
    string code,
    CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return Challenge();

            if (string.IsNullOrWhiteSpace(code))
            {
                TempData["error"] = "Please enter a promotion code.";
                return RedirectToAction(nameof(Index));
            }

            var promotion = await _promotionRepository.GetOneAsync(
                p => p.Code == code.Trim() && p.IsValid);

            if (promotion is null)
            {
                TempData["error"] = "Invalid promotion code.";
                return RedirectToAction(nameof(Index));
            }

            var cartItem = await _cartRepository.GetOneAsync(
     c => c.ApplicationUserId == user.Id
       && c.ProductId == promotion.ProductId,
     includes: [c => c.Product]);

            if (cartItem is null)
            {
                TempData["error"] =
                    "This promotion does not apply to products in your cart.";

                return RedirectToAction(nameof(Index));
            }
            if (promotion is not null)
            {
                cartItem.Price -= (cartItem.Product.Price * promotion.Discount / 100);
                await _cartRepository.CommitAsync(cancellationToken);
            }
            // Persist the applied promotion here.
            // Do not subtract the discount from cartItem.Price repeatedly.

            TempData["success"] = "Promotion applied successfully!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddToCart(int productId, int count, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if (cart != null)
            {
                cart.count += count;
                await _cartRepository.CommitAsync(cancellationToken);
                TempData["success"] = "Product added to cart successfully!";
                return RedirectToAction("Index", "Home");
            }
            await _cartRepository.AddAsync(new()
            {

                ProductId = productId,
                ApplicationUserId = user.Id,
                count = count,
                Price = (await _productRepository.GetOneAsync(e => e.ID == productId)!).Price

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
        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetAsync(c => c.ApplicationUserId == user.Id,includes: [c => c.Product]);
            var options = new SessionCreateOptions
            {
                AllowedPaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),  
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/cancel",
            };



            foreach (var item in cart)
            {

                options.LineItems.Add(

                       new SessionLineItemOptions
                       {
                           PriceData = new SessionLineItemPriceDataOptions
                           {
                               Currency = "egp",
                               ProductData = new SessionLineItemPriceDataProductDataOptions
                               {
                                   Name = item.Product.Name,
                                   Description = item.Product.Description,
                                   //etc.. any data you need showing in cart payment 
                               },
                               UnitAmount =(long) item.Price *100,
                           },
                           Quantity = item.count,
                       }
                    ); 
            }




            var service = new SessionService(); 
            var session = service.Create(options);
            return Redirect(session.Url);

        }
    }
}
