using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Entities.Repositories
{
    public interface IGenericRepository<T> where T :class
    {
        //_context.Categories.Include().ToList();
        //_context.Categories.Where().ToList();
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? perdicate = null, string? Includeword =null);
        
        T GetFirstorDefault(Expression<Func<T, bool>>? perdicate = null, string? Includeword = null);

        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entites);

    }
}
