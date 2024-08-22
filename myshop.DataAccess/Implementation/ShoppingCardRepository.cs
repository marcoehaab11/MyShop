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
    public class ShoppingCardRepository : GenericRepository<ShoppingCard>, IShoppingCardRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCardRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public int DecreaseCount(ShoppingCard shoppongcard, int count)
        {
            shoppongcard.Count -= count;
            return shoppongcard.Count;
        }

        public int IncreaseCount(ShoppingCard shoppongcard, int count)
        {
            shoppongcard.Count += count;
            return shoppongcard.Count;
        }
    }
}
