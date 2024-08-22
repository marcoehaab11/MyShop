using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;


namespace myshop.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var categories = _unitOfWork.Category.GetAll();
            return View(categories);

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {   //Server Side Validation
            if (ModelState.IsValid)
            {
                // _context.Categories.Add(category);
                // _context.SaveChanges();
                _unitOfWork.Category.Add(category);
                _unitOfWork.Complete();
                TempData["Create"] = "Data Has Create Succesfully";

                return RedirectToAction("Index");
            }
            return View(category);

        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            var category = _unitOfWork.Category.GetFirstorDefault(x => x.Id == id);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {   //Server Side Validation
            if (ModelState.IsValid)
            {
                //_context.Categories.Update(category);
                //_context.SaveChanges();
                _unitOfWork.Category.Update(category);
                _unitOfWork.Complete();

                TempData["Update"] = "Data Has Updated Succesfully";

                return RedirectToAction("Index");
            }
            return View(category);

        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }


            var category = _unitOfWork.Category.GetFirstorDefault(x => x.Id == id);

            return View(category);
        }
        [HttpPost]
        public IActionResult DeleteCategory(int? id)
        {   //Server Side Validation

            var cate = _unitOfWork.Category.GetFirstorDefault(x => x.Id == id);

            if (cate == null)
            {
                NotFound();
            }
            //_context.Categories.Remove(cate);
            //_context.SaveChanges();
            _unitOfWork.Category.Remove(cate);
            _unitOfWork.Complete();
            TempData["Delete"] = "Data Has Deleted Succesfully";
            return RedirectToAction("Index");



        }
    }
}
