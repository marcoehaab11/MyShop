using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace myshop.DataAccess.Implementation
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
		public void Update(OrderHeader orderheader)
        {
            _context.OrderHeaders.Update(orderheader);
        }



        public  void UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus)
        {
            var orderfromDb=_context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if(orderfromDb != null)
            {
                orderfromDb.OrderStatus = OrderStatus;
                if (PaymentStatus != null)
                {
                    orderfromDb.PaymentStatus = PaymentStatus;
                }
            }
        }
       


    }
}
