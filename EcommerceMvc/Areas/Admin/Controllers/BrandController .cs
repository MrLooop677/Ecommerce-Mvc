using EcommerceMvc.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace EcommerceMvc.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class BrandController : Controller
    {
        //ApplicationDbContext _context = new();
       private readonly IRepository<Brand> _brandRepository;//= new();

        public BrandController(IRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            //var Brands = _context.Brands.AsQueryable().AsNoTracking();
            var Brands =await _brandRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            return View(Brands.Select(e => new
            {
                e.ID,
                e.Name,
                e.Description,
                e.Status,
                e.Img

            }).AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandVM brandVM,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(brandVM);

            //Brand brand = new()
            //{
            //    Name = brandVM.Name,
            //    Description = brandVM.Description,
            //    Status = brandVM.Status,
            //};
            //using mapster to mapping between createBrandVM and brand
            Brand brand = brandVM.Adapt<Brand>();
            if (brandVM.Img is not null && brandVM.Img.Length > 0)
            {
                //create and set img
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(brandVM.Img.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    brandVM.Img.CopyTo(stream);
                }
                brand.Img = fileName;
            }
            //_context.Brands.Add(brand);
            //_context.SaveChanges();
            await _brandRepository.AddAsync(brand, cancellationToken);
            await _brandRepository.CommitAsync(cancellationToken);
            //Response.Cookies.Append("success-notification", "Brand created successfully!");
            TempData["success-notification"] = "Brand created successfully!";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            //var selectedbrand = _context.Brands.FirstOrDefault(c => c.ID == id);
            var selectedbrand = await _brandRepository.GetOneAsync(c => c.ID == id, cancellationToken: cancellationToken);

            if (selectedbrand == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            //return View(new UpdateBrandVM
            //{
            //    Id = selectedbrand.ID,
            //    Name = selectedbrand.Name,
            //    Description = selectedbrand.Description,
            //    Status = selectedbrand.Status,
            //    Img = selectedbrand.Img,
            //});
            //using mapster to mapping between selectedbrand and updateBrandVM
            var updatedBrandVM = selectedbrand.Adapt<UpdateBrandVM>();
            return View(updatedBrandVM);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateBrandVM updateBrandVM,CancellationToken cancellationToken)
        {
            if(!ModelState.IsValid) 
                return View(updateBrandVM);
            //var selectedbrand = _context.Brands.AsNoTracking().FirstOrDefault(c => c.ID == updateBrandVM.Id);
            var selectedbrand =await _brandRepository.GetOneAsync(c => c.ID == updateBrandVM.ID,tracked:false, cancellationToken: cancellationToken);
            if (selectedbrand is null)
                return RedirectToAction("NotFoundPage", "Home");
            //Brand brand = new()
            //{
            //    ID = updateBrandVM.Id,
            //    Name = updateBrandVM.Name,
            //    Status = updateBrandVM.Status,
            //    Description = updateBrandVM.Description,
            //    Img=selectedbrand.Img
            //};
            //using mapster to mapping between updateBrandVM and brand
            Brand brand =updateBrandVM.Adapt<Brand>();
            if (updateBrandVM.NewImg is not null)
            {
                if (updateBrandVM.NewImg is not null && updateBrandVM.NewImg.Length > 0)
                {
                    //create and set img
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updateBrandVM.NewImg.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        updateBrandVM.NewImg.CopyTo(stream); 
                    }

                    //Remove old img from wwwroot
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", selectedbrand.Img);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);

                    //Save New Img
                    brand.Img = fileName;
                }
            }
            else
            {
                brand.Img = selectedbrand.Img;
            }

            //_context.Brands.Update(brand);
            //_context.SaveChanges();
            _brandRepository.Update(brand);
            await _brandRepository.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Brand updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        //[HttpGet]

        public async Task<IActionResult> Delete(int id)
        {
            //var deletedItem = _context.Brands.FirstOrDefault(c => c.ID == id);
            var deletedItem = await _brandRepository.GetOneAsync(c => c.ID == id);
            if (deletedItem == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }


            //_context.Brands.Remove(deletedItem);
            _brandRepository.Delete(deletedItem);

            //_context.SaveChanges();
            await _brandRepository.CommitAsync();
            TempData["success-notification"] = "Brand deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
