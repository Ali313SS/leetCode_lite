using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AJudge.Infrastructure.Repositories
{
    public class BaseRepoitory<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        public BaseRepoitory(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T?> Create(T item)
        {
            await _context.Set<T>().AddAsync(item);
            return item;    
        }

        public async Task<bool> Delete<K>(K id)
        {
            T? item = await GetById(id);
            if (item == null)
                return false;

            _context.Set<T>().Remove(item);
            return true;
        }
        public T? Update(T item)
        {
            _context.Set<T>().Update(item);
            return item;
        }
            
           


        public async Task<IEnumerable<T>?> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

      

   //      public async Task<T?> GetById(int id)
   //  {
   //      return await _context.Set<T>().FindAsync(id);
   //  }

        public async Task<T?> GetById<K>(K id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetById<K>(K id,bool track=true)
        {
            IQueryable<T> query = GetQuery();
            if (!track)
            {
                query = query.AsNoTracking(); 
            }
           

            var keyProperty = typeof(T).GetProperties()
                                 .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any());

            if (keyProperty == null)
            {
                keyProperty = typeof(T).GetProperties()
                                       .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                                                            p.Name.Equals(typeof(T).Name + "Id", StringComparison.OrdinalIgnoreCase));

                if (keyProperty == null)
                {
                    throw new InvalidOperationException("Entity does not have a primary key property.");
                }
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<object>(e,keyProperty.Name).Equals(id));
        }

        //  public async Task<T?> GetById(Guid id)
        //  {
        //      return await _context.Set<T>().FindAsync(id);
        //  }





        public IQueryable<T> GetQuery()
        {
            return _context.Set<T>().AsQueryable();
        }

        public async Task<T?> GetSpecific(Expression<Func<T, bool>> predicate, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query=query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> GetById(Expression<Func<T,bool>> predict, string[]? includes)
        {
            IQueryable<T> query = GetQuery();

            if (includes != null)
            {
                foreach(var include in includes)
                {
                    query=query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(predict);
        }




        public async Task<List<T>> GetAllUsingPredict(Expression<Func<T, bool>> predict, string[]? includes)
        {
            IQueryable<T> query = GetQuery();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.Where(predict).ToListAsync();
        }
    }
        

}
