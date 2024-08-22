using myshop.DataAccess.Data;
using myshop.Entities.Models;
using myshop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.DataAccess.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
           var oldproduct=_context.Products.FirstOrDefault(x=>x.Id == product.Id);
           if (oldproduct != null)
           {
                oldproduct.Name= product.Name;
                oldproduct.Description= product.Description;
                oldproduct.Price= product.Price;
                oldproduct.Img= product.Img;
                oldproduct.CategoryId= product.CategoryId;
            }
        }
    }
}
