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

        public Task AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
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

        public Task<List<T>> GetSpecificList(Expression<Func<T, bool>> predicate, string[]? includes = null)
        {
            throw new NotImplementedException();
        }
    }
        

}
