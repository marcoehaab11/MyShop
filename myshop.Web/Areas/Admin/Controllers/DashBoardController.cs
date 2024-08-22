using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Repositories;
using Utilities;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class DashBoardController : Controller
    {   private readonly IUnitOfWork _unitOfWork;

        public DashBoardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            ViewBag.Orders = _unitOfWork.OrderHeader.GetAll().Count();
            ViewBag.ApprovedOrders = _unitOfWork.OrderHeader.GetAll(u=>u.OrderStatus==$"{SD.Approve}").Count();
            ViewBag.Users=_unitOfWork.ApplicationUser.GetAll().Count(); 
            ViewBag.Products=_unitOfWork.ApplicationUser.GetAll().Count();  
            

            return View();
        }
    }
}
