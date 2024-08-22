using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using myshop.Entities.ViewModels;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Security.Claims;
using Utilities;

namespace myshop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCardVM ShoppingCardVM { get; set; }
        public decimal Total { get; set; }
        public CardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimIdentuty = (ClaimsIdentity)User.Identity;
            var claim = claimIdentuty.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCardVM = new ShoppingCardVM()
            {
                CardList = _unitOfWork.ShoppingCard
                .GetAll(u => u.ApplicationUserId == claim.Value, Includeword: "product").ToList(),
				OrderHeader = new()

			};
            foreach (var item in ShoppingCardVM.CardList)
            {
                ShoppingCardVM.OrderHeader.TotalPrice += (item.Count * item.product.Price);
            }


            return View(ShoppingCardVM);
        }
        [HttpGet]
        public IActionResult Summary()
        {
            var claimIdentuty = (ClaimsIdentity)User.Identity;
            var claim = claimIdentuty.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCardVM = new ShoppingCardVM()
            {
                CardList = _unitOfWork.ShoppingCard
                .GetAll(u => u.ApplicationUserId == claim.Value, Includeword: "product").ToList(),
                OrderHeader = new()

            };
            ShoppingCardVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                .GetFirstorDefault(x => x.Id == claim.Value);
            ShoppingCardVM.OrderHeader.Name = ShoppingCardVM.OrderHeader.ApplicationUser.Name;
            ShoppingCardVM.OrderHeader.Address = ShoppingCardVM.OrderHeader.ApplicationUser.Address;
            ShoppingCardVM.OrderHeader.City = ShoppingCardVM.OrderHeader.ApplicationUser.City;
            ShoppingCardVM.OrderHeader.Phone = ShoppingCardVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var item in ShoppingCardVM.CardList)
            {
                ShoppingCardVM.OrderHeader.TotalPrice += (item.Count * item.product.Price);
            }


            return View(ShoppingCardVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult POSTSummary(ShoppingCardVM shoppingCardVM)
        {
            var claimIdentuty = (ClaimsIdentity)User.Identity;
            var claim = claimIdentuty.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCardVM.CardList = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value,
                Includeword: "product");
            shoppingCardVM.OrderHeader.OrderStatus = SD.Pending;
            shoppingCardVM.OrderHeader.PaymentStatus = SD.Pending;
            shoppingCardVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCardVM.OrderHeader.ApplicationUserId = claim.Value;


            foreach (var item in shoppingCardVM.CardList)
            {
                shoppingCardVM.OrderHeader.TotalPrice += (item.Count * item.product.Price);
            }

            _unitOfWork.OrderHeader.Add(shoppingCardVM.OrderHeader);
            _unitOfWork.Complete();

            foreach (var item in shoppingCardVM.CardList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderId = shoppingCardVM.OrderHeader.Id,
                    Price = item.product.Price,
                    Count = item.Count,
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Complete();
            }
            var domain = "https://localhost:44356/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain+$"Customer/Card/OrderConfirmation/{shoppingCardVM.OrderHeader.Id}",
                CancelUrl = domain+$"customer/cart/index",
            };
            foreach (var item in shoppingCardVM.CardList)
            {
                var sessionluneoption = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.product.Price*100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.product.Name,
                        },
                    },
                    Quantity=item.Count
                };
                options.LineItems.Add(sessionluneoption);
            }   
            var service = new SessionService();
            Session session = service.Create(options);
            shoppingCardVM.OrderHeader.SessionId=session.Id;

            _unitOfWork.Complete();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
           
        }


        [HttpGet]
        public IActionResult OrderConfirmation(int id)
        {   
            var order=_unitOfWork.OrderHeader.GetFirstorDefault(x=>x.Id==id);
            var service = new SessionService();
            Session session = service.Get(order.SessionId);
            if (session.PaymentStatus.ToLower()=="paid")
            {
				order.PaymentIntentId = session.PaymentIntentId;
				_unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.Approve, SD.Approve);
                _unitOfWork.Complete();
            }
            List<ShoppingCard> shoppingcarts=_unitOfWork.ShoppingCard.
                GetAll(u=>u.ApplicationUserId==order.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCard.RemoveRange(shoppingcarts);
            _unitOfWork.Complete();

            return View(id);
        }
        public IActionResult Plus(int cartid)
        {
            var shoppingcard=_unitOfWork.ShoppingCard.GetFirstorDefault(x=>x.Id==cartid);
            _unitOfWork.ShoppingCard.IncreaseCount(shoppingcard, 1);
            _unitOfWork.Complete();

            HttpContext.Session.SetInt32(SD.SessionKey,
                _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == shoppingcard.ApplicationUserId).ToList().Count());
            return RedirectToAction("Index");
         

        }
		public IActionResult Minus(int cartid)
		{
			var shoppingcard = _unitOfWork.ShoppingCard.GetFirstorDefault(x => x.Id == cartid);
		    if(shoppingcard.Count <= 1)
            {
                _unitOfWork.ShoppingCard.Remove(shoppingcard);
                var count =_unitOfWork.ShoppingCard.GetAll(x=>x.ApplicationUserId==shoppingcard.ApplicationUserId).ToList().Count()-1;
                HttpContext.Session.SetInt32(SD.SessionKey,count);
            }
            else
            {
				_unitOfWork.ShoppingCard.DecreaseCount(shoppingcard, 1);
			}
		    _unitOfWork.Complete();

			return RedirectToAction("Index");
		}
		public IActionResult Remove(int cartid)
		{
			var shoppingcard = _unitOfWork.ShoppingCard.GetFirstorDefault(x => x.Id == cartid);
			_unitOfWork.ShoppingCard.Remove(shoppingcard);
			_unitOfWork.Complete();
            var count = _unitOfWork.ShoppingCard.GetAll(x => x.ApplicationUserId == shoppingcard.ApplicationUserId).ToList().Count();
            HttpContext.Session.SetInt32(SD.SessionKey, count);


            return RedirectToAction("Index");
		}
	}
}
