using myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Entities.Repositories
{
    public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
    {
        void Update (OrderHeader orderheader);
        void UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus);

    }
}
