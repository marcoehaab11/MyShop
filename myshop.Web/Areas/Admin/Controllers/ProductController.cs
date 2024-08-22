using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;


namespace myshop.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();

        }
        public IActionResult GetData()
        {
            var Products = _unitOfWork.Product.GetAll(Includeword: "Category");
            return Json(new { data = Products });

        }
        [HttpGet]
        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                Categorylist = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM, IFormFile file)
        {   //Server Side Validation
            if (ModelState.IsValid)
            {

                string Rootpath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var Uplaod = Path.Combine(Rootpath, @"img\Products");
                    var ext = Path.GetExtension(file.FileName);

                    using (var filestrem = new FileStream(Path.Combine(Uplaod, filename + ext), FileMode.Create))
                    {
                        file.CopyTo(filestrem);
                    }
                    productVM.Product.Img = @"img\Products\" + filename + ext;
                }


                // _context.Categories.Add(category);
                // _context.SaveChanges();
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Complete();
                TempData["Create"] = "Data Has Create Succesfully";

                return RedirectToAction("Index");
            }
            return View(productVM);

        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            ProductVM productVM = new ProductVM()
            {
                Product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == id),
                Categorylist = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {   //Server Side Validation
            if (ModelState.IsValid)
            {

                string Rootpath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var Uplaod = Path.Combine(Rootpath, @"img\Products");
                    var ext = Path.GetExtension(file.FileName);
                    if (productVM.Product.Img != null)
                    {
                        var oldimg = Path.Combine(Rootpath, productVM.Product.Img.TrimStart('\\'));
                        if (System.IO.File.Exists(oldimg))
                        {
                            System.IO.File.Delete(oldimg);
                        }
                    }
                    using (var filestrem = new FileStream(Path.Combine(Uplaod, filename + ext), FileMode.Create))
                    {
                        file.CopyTo(filestrem);
                    }
                    productVM.Product.Img = @"img\Products\" + filename + ext;
                }


                // _context.Categories.Add(category);
                // _context.SaveChanges();
                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Complete();
                TempData["Update"] = "Data Has Updated Succesfully";
                return RedirectToAction("Index");
            }
            return View(productVM);

        }
      
        [HttpDelete]
        public IActionResult Delete(int? id)
        {   //Server Side Validation

            var product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == id);

            if (product == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            string Rootpath = _webHostEnvironment.WebRootPath;
            if (product.Img != null)
            {
                var oldimg = Path.Combine(Rootpath, product.Img.TrimStart('\\'));
                if (System.IO.File.Exists(oldimg))
                {
                    System.IO.File.Delete(oldimg);
                }
            }
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Complete();
            return Json(new { success = true, message = "File has been Deleted" });



        }


    }
    
}
