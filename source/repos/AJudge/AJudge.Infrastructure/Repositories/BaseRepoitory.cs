using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task Delete(int id)
        {
            T? item = await GetById(id);
            if (item == null)
                return;
            _context.Set<T>().Remove(item);
            
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

      

            public async Task<T?> GetById(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

      

       

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

      
    }
        

}
