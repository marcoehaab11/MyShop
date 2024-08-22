using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using System.Security.Claims;
using Utilities;
using X.PagedList.Extensions;

namespace myshop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Index(int? page)
        {
            var PageNumber = page ?? 1;
            var PageSize = 8;


            var products =_unitOfWork.Product.GetAll().ToPagedList(PageNumber, PageSize);
            return View(products);
        }
        public IActionResult Details(int ProductId)
        {

            ShoppingCard obj = new ShoppingCard()
            {   ProductId= ProductId,
                product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == ProductId, Includeword: "Category")
                ,Count=1
                };
            return View(obj);

        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCard shoppinCard)
        {
            var claimIdentuty=(ClaimsIdentity)User.Identity;
            var claim=claimIdentuty.FindFirst(ClaimTypes.NameIdentifier);
            shoppinCard.ApplicationUserId = claim.Value;


            ShoppingCard cardobj = _unitOfWork.ShoppingCard.GetFirstorDefault(
                u => u.ApplicationUserId == claim.Value &&
                u.ProductId == shoppinCard.ProductId);
            if (cardobj == null)
            {
                _unitOfWork.ShoppingCard.Add(shoppinCard);
                _unitOfWork.Complete();

                HttpContext.Session.SetInt32(SD.SessionKey,
                    _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count());

            }
            else
            {
                _unitOfWork.ShoppingCard.IncreaseCount(cardobj, shoppinCard.Count);
                _unitOfWork.Complete();

            }
            return RedirectToAction("Index");

        



         

        }
    }
}
