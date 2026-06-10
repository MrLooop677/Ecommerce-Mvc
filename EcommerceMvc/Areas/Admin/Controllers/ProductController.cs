using EcommerceMvc.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace EcommerceMvc.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {
        ApplicationDbContext _context = new();
        ProductRepository _productRepository = new();
        Repository<Category> _categoryRepository = new();
        Repository<Brand> _brandRepository = new();
        Repository<ProductSubImg> _productSubImagesRepository = new();
        Repository<ProductColor> _productColorsRepository = new();

        public async Task<IActionResult> Index(FilterProductVM FilterProductVM, CancellationToken cancellationToken, int page = 1)
        {
            const decimal discount = 50;
            const double pageSize = 5;
            //var Products = _context.Products.AsQueryable().AsNoTracking();
            var Products = await _productRepository.GetAsync(includes: [p => p.Category, p => p.Brand] , tracked: false, cancellationToken: cancellationToken);
            if (FilterProductVM.name != null)
            {

                Products = Products.Where(p => p.Name.Contains(FilterProductVM.name.Trim()));
                ViewBag.Name = FilterProductVM.name;
            }
            if (FilterProductVM.minPrice != null)
            {
                Products = Products.Where(p => p.Price - (p.Price * p.Discount / 100) > FilterProductVM.minPrice);
                ViewBag.MinPrice = FilterProductVM.minPrice;

            }
            if (FilterProductVM.maxPrice != null)
            {
                Products = Products.Where(p => p.Price - (p.Price * p.Discount / 100) < FilterProductVM.maxPrice);
                ViewBag.MaxPrice = FilterProductVM.maxPrice;

            }
            if (FilterProductVM.categoryId != null)
            {
                Products = Products.Where(p => p.CategoryId == FilterProductVM.categoryId);
                ViewBag.CategoryId = FilterProductVM.categoryId;
            }
            if (FilterProductVM.brandId != null)
            {
                Products = Products.Where(p => p.BrandId == FilterProductVM.brandId);
                ViewBag.BrandId = FilterProductVM.brandId;
            }

            if (FilterProductVM.lessQuantity)
            {
                Products = Products.OrderBy(p => p.Quantity);
                ViewBag.LessQuantity = FilterProductVM.lessQuantity;
            }

            //var categories = _context.Categories.AsNoTracking();
            //var brands = _context.Brands.AsNoTracking();
            var categories = await _categoryRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            var brands = await _brandRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            ViewData["Categories"] = categories.AsEnumerable();
            ViewBag.Brands = brands.AsEnumerable();

            //pagination
            var totalCount = Products.Count();

            ViewBag.totalPage = Math.Ceiling(totalCount / pageSize);
            ViewBag.currentPage = page;
            Products = Products.Skip((page - 1) * (int)pageSize).Take((int)pageSize);
            return View(Products.Select(e => new
            {
                e.ID,
                e.Name,
                e.Description,
                e.Status,
                e.MainImage,
                e.Price,
                e.Quantity,
                CategoryName = e.Category.Name,
                BrandName = e.Brand.Name

            }).AsEnumerable());
        }
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {

            var categories = await _categoryRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            var brands = await _brandRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);

            return View(new ProductVM
            {
                Categories = categories.AsEnumerable(),
                Brands = brands.AsEnumerable()
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile image, List<IFormFile>? subImages, string[] colors,CancellationToken cancellationToken)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (image is not null && image.Length > 0)
                {
                    //create and set img
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        image.CopyTo(stream);
                    }
                    product.MainImage = fileName;
                }
                //var productCreated = _context.Products.Add(product);
                //_context.SaveChanges();
                var productCreated = await _productRepository.AddAsync(product, cancellationToken);
                await _productRepository.CommitAsync();
                if (subImages is not null && subImages.Count > 0)
                {

                    foreach (var img in subImages)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            img.CopyTo(stream);
                        }
                        await _productSubImagesRepository.AddAsync(new ProductSubImg
                        {
                            Img = fileName,
                            ProductId = productCreated.ID
                        }, cancellationToken);
                    }
                    await _productSubImagesRepository.CommitAsync(cancellationToken);
                }
                if (colors.Any())
                {
                    var ProductColors = await _productColorsRepository.GetAsync(cancellationToken: cancellationToken);
                    foreach (var color in colors)
                    {

                        await _productColorsRepository.AddAsync(new ProductColor
                        {
                            Color = color,
                            ProductId = productCreated.ID,
                        }, cancellationToken);
                    }
                    await _productColorsRepository.CommitAsync(cancellationToken);

                }
                //Response.Cookies.Append("success-notification", "Product created successfully!");
                TempData["success-notification"] = "Product created successfully!";
                transaction.Commit();
            }
            catch (Exception ex)
            {
                TempData["error-notification"] = "Error occurred while creating product!";
                transaction.Rollback();

            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            //var selectedproduct = _context.Products.Include(p => p.ProductSubImages).Include(p => p.ProductColors).FirstOrDefault(c => c.ID == id);
            var selectedproduct =await _productRepository.GetOneAsync(includes: [p => p.ProductSubImages, p => p.ProductColors],expression: p => p.ID == id,cancellationToken: cancellationToken);
            if (selectedproduct == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            var categories =await _categoryRepository.GetAsync(cancellationToken: cancellationToken,tracked:false);
            var brands = await _brandRepository.GetAsync(cancellationToken: cancellationToken,tracked:false);

            return View(new ProductVM
            {
                Categories = categories.AsEnumerable(),
                Brands = brands.AsEnumerable(),
                Product = selectedproduct
            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile? image, int id, string[] colors, List<IFormFile>? SubImgs, CancellationToken cancellationToken)
        {
            //var selectedproduct = _context.Products.Include(p => p.ProductSubImages).Include(p => p.ProductColors).FirstOrDefault(c => c.ID == product.ID);
            var selectedproduct =await _productRepository.GetOneAsync(includes: [p => p.ProductSubImages, p => p.ProductColors],expression: p => p.ID == product.ID,cancellationToken: cancellationToken);
            if (selectedproduct is null)
                return RedirectToAction("NotFoundPage", "Home");

            if (image is not null)
            {
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
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", selectedproduct.MainImage);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);

                    //Save New Img
                    product.MainImage = fileName;
                }
            }
            else
            {
                product.MainImage = selectedproduct.MainImage;
            }

            if (SubImgs is not null && SubImgs.Count > 0)
            {

                foreach (var img in SubImgs)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        img.CopyTo(stream);
                    }
                    await _productSubImagesRepository.AddAsync(new ProductSubImg
                    {
                        Img = fileName,
                        ProductId = selectedproduct.ID
                    }, cancellationToken);
                    await _productSubImagesRepository.CommitAsync(cancellationToken);
                }
            }

            if (colors.Any())
            {
                var ProductColors = await _productColorsRepository.GetAsync(cancellationToken: cancellationToken, tracked: false);
                foreach (var item in selectedproduct.ProductColors)
                {

                    _productColorsRepository.Delete(item);
                }

                foreach (var color in colors)
                {

                    await _productColorsRepository.AddAsync(new ProductColor
                    {
                        Color = color,
                        ProductId = selectedproduct.ID,
                    }, cancellationToken);
                }
                await _productColorsRepository.CommitAsync(cancellationToken);
            }

            // best bractes for update product
            //_context.Products.Update(product);
            selectedproduct.Name = product.Name;
            selectedproduct.Description = product.Description;
            selectedproduct.Price = product.Price;
            selectedproduct.Quantity = product.Quantity;
            selectedproduct.BrandId = product.BrandId;
            selectedproduct.CategoryId = product.CategoryId;
            selectedproduct.Discount = product.Discount;
            selectedproduct.Status = product.Status;
            selectedproduct.MainImage = product.MainImage;
            Console.WriteLine(selectedproduct.MainImage);
            await  _productRepository.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Product updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteSubImg(int productId, string img, CancellationToken cancellationToken)
        {
            var productSubImg = await _productSubImagesRepository.GetOneAsync(expression: e => e.Img == img && e.ProductId == productId, cancellationToken: cancellationToken);
            if (productSubImg != null)
            {
                 _productSubImagesRepository.Delete(productSubImg);
                //Remove old subimages from wwwroot\\product_images
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", productSubImg.Img);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

            }
            else
                return RedirectToAction("NotFoundPage", "Home");

            await _productSubImagesRepository.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(Edit), new { id = productId });
        }
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deletedItem = await _productRepository.GetOneAsync(expression: c => c.ID == id, cancellationToken: cancellationToken);
            if (deletedItem == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", deletedItem.MainImage);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
            foreach (var subimg in deletedItem.ProductSubImages)
            {


                var subimgoldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", subimg.Img);
                if (System.IO.File.Exists(subimgoldPath))
                    System.IO.File.Delete(subimgoldPath);
            }

            //_context.ProductSubImages.RemoveRange(deletedItem.ProductSubImages);
            _productSubImagesRepository.DeleteRange(deletedItem.ProductSubImages);

            //_context.ProductColors.RemoveRange(deletedItem.ProductColors);
            _productColorsRepository.DeleteRange(deletedItem.ProductColors);
            //_context.Products.Remove(deletedItem);
            _productRepository.Delete(deletedItem);

            //_context.SaveChanges();
           await _productRepository.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Product deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
