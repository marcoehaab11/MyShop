using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using Stripe;
using System.Collections.Generic;
using Utilities;

namespace myshop.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]

    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM {  get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult GetData()
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = _unitOfWork.OrderHeader.GetAll(Includeword: "ApplicationUser");
            return Json(new {data=orderHeaders});
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader=_unitOfWork.OrderHeader.GetFirstorDefault(u=>u.Id==id,Includeword:"ApplicationUser")
                ,OrderDetail= _unitOfWork.OrderDetail.GetAll(u => u.OrderId== id, Includeword: "Product")
            };
            return View(orderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails() 
        {
            var orderformDb= _unitOfWork.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderformDb.Name= OrderVM.OrderHeader.Name; 
            orderformDb.Phone= OrderVM.OrderHeader.Phone; 
            orderformDb.Address= OrderVM.OrderHeader.Address; 
            orderformDb.City= OrderVM.OrderHeader.City;

            if (OrderVM.OrderHeader.Carrier !=null)
            {
                orderformDb.Carrier= OrderVM.OrderHeader.Carrier;   
            }
            if (OrderVM.OrderHeader.TrackingNumber != null)
            {
                orderformDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderformDb);
            _unitOfWork.Complete();


			TempData["Update"] = "Item has Created Successfully";
			return RedirectToAction("Details","Order",new { id = orderformDb.Id});
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartProccess()
		{
            _unitOfWork.OrderHeader.UpdateOrderStatus(OrderVM.OrderHeader.Id, SD.Proccessing, null);
            _unitOfWork.Complete();
		

			TempData["Update"] = "Order Status has Updated Successfully";
			return RedirectToAction("Details", "Order", new { id = OrderVM.OrderHeader.Id });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartShip()
		{
            var orderfrdb = _unitOfWork.OrderHeader.GetFirstorDefault(u=>u.Id==OrderVM.OrderHeader.Id);
			orderfrdb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderfrdb.Carrier = OrderVM.OrderHeader.Carrier;
            orderfrdb.OrderStatus = SD.Shipped;
            orderfrdb.ShippingDate=DateTime.Now;
            

			_unitOfWork.OrderHeader.Update(orderfrdb);
            _unitOfWork.Complete();


			TempData["Update"] = "Order Status has Shipped Successfully";
			return RedirectToAction("Details", "Order", new { id = OrderVM.OrderHeader.Id });
		}
        [HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
            var orderfrdb = _unitOfWork.OrderHeader.GetFirstorDefault(u=>u.Id==OrderVM.OrderHeader.Id);
            if (orderfrdb.PaymentStatus==SD.Approve)
            {
                var op = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent=orderfrdb.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(op);
                _unitOfWork.OrderHeader.UpdateOrderStatus(orderfrdb.Id, SD.Canelled, SD.Canelled);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateOrderStatus(orderfrdb.Id, SD.Canelled, SD.Canelled);

			}
            _unitOfWork.Complete();

			TempData["Update"] = "Order has Canelled Successfully";
			return RedirectToAction("Details", "Order", new { id = OrderVM.OrderHeader.Id });
		}


	}
}
