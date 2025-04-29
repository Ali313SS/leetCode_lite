using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.RepoContracts
{
    public interface IBaseRepository<T>where T : class
    {
        Task<T?> GetSpecific(Expression<Func<T, bool>> predicate, string[]? includes = null);
        Task<IEnumerable<T>?> GetAll();
      
        Task<T?> Create(T item);
        T? Update(T item);
        Task Delete(int id);



        Task<T?> GetById(int id);
        IQueryable<T> GetQuery();
    }
}
