using myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Entities.Repositories
{
    public interface IShoppingCardRepository : IGenericRepository<ShoppingCard>
    {

        int IncreaseCount(ShoppingCard shoppongcard, int count);
        int DecreaseCount(ShoppingCard shoppongcard, int count);
    }
}
