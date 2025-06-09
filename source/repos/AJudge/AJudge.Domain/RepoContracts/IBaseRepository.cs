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
     //   Task<T> GetSpecifics(Expression<Func<T, bool>> predicate);

        Task<List<T>> GetAllApply(Expression<Func<T, bool>> predicate, string[]? includes = null);
        Task<IEnumerable<T>?> GetAll();
      
        Task<T?> Create(T item);
        T? Update(T item);
        Task<bool> Delete<K>(K id);
        Task<T?> GetById<K>(K id, bool track = true);
        Task<T?> GetById<K>(K id);
        //Task<T?> GetById(Guid id);
        Task<List<T>> GetAllUsingPredict(Expression<Func<T, bool>> predict, string[]? includes);
       // Task<T?> GetById(int id);
        Task<T?> GetById(Expression<Func<T, bool>> predict, string[]? includes);
        IQueryable<T> GetQuery();

        IQueryable<T> ApplySort(IQueryable<T> query, string sortBy, bool isAssending = true);
        IQueryable<T> ApplyFilter(IQueryable<T> query, string filterBy, string filterValue);
    }
}
