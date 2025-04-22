//using AJudge.Domain.Entities;
//using AJudge.Domain.RepoContracts;
//using AJudge.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace AJudge.Infrastructure.Repositories
//{
//    public class ProblemRepository : BaseRepoitory<Problem>, IProblemRepository
//    {

//        public ProblemRepository(ApplicationDbContext context) : base(context)
//        {
//        }
//        public Task<List<Problem>>? GetAllInPage(string? sortBy, bool isAssending = true, int pageNumber = 1, int pageSize = 100)
//        {
//            IQueryable<Problem> query = _context.Problems;


//            if (string.IsNullOrEmpty(sortBy) || typeof(Problem).GetProperty(sortBy) == null)
//            {
//                sortBy = "ProblemId";
//            }


//                var parameter = Expression.Parameter(typeof(Problem), "x");


//                var property = Expression.Property(parameter, sortBy);

//                var propertyType = property.Type;

//                var lambda = Expression.Lambda(property, parameter);

//                string methodName = isAssending ? "OrderBy" : "OrderByDescending";

//                var result = Expression.Call(

//                    typeof(Queryable), 
//                    methodName,    
//                    new Type[] { typeof(Problem), propertyType }, 
//                    query.Expression, 
//                    Expression.Quote(lambda)
//                    );



//                query = query.Provider.CreateQuery<Problem>(result);


//            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);



//            return query.ToListAsync();
//        }
//    }
//}

using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AJudge.Infrastructure.Repositories
{
    public class ProblemRepository : BaseRepository<Problem>, IProblemRepository
    {
        public ProblemRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Problem>> GetAllInPage(string sortBy, bool isAssending = true, int pageNumber = 1, int pageSize = 100)
        {
            var query = _context.Problems.AsQueryable();
            switch (sortBy?.ToLower())
            {
                case "name":
                    query = isAssending ? query.OrderBy(p => p.ProblemName) : query.OrderByDescending(p => p.ProblemName);
                    break;
                case "rating":
                    query = isAssending ? query.OrderBy(p => p.Rating) : query.OrderByDescending(p => p.Rating);
                    break;
                default:
                    query = isAssending ? query.OrderBy(p => p.ProblemId) : query.OrderByDescending(p => p.ProblemId);
                    break;
            }
            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }

    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public IQueryable<T> GetAll()
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
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetSpecificList(Expression<Func<T, bool>> predicate, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(predicate).ToListAsync();
        }
    }
}