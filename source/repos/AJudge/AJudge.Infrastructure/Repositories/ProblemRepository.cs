using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Infrastructure.Repositories
{
    public class ProblemRepository : BaseRepoitory<Problem>, IProblemRepository
    {

        public ProblemRepository(ApplicationDbContext context) : base(context)
        {
        }
        public Task<List<Problem>>? GetAllInPage(string? sortBy, bool isAssending = true, int pageNumber = 1, int pageSize = 100)
        {
            IQueryable<Problem> query = _context.Problems;


            if (string.IsNullOrEmpty(sortBy) || typeof(Problem).GetProperty(sortBy) == null)
            {
                sortBy = "ProblemId";
            }

           
                var parameter = Expression.Parameter(typeof(Problem), "x");


                var property = Expression.Property(parameter, sortBy);

                var propertyType = property.Type;

                var lambda = Expression.Lambda(property, parameter);

                string methodName = isAssending ? "OrderBy" : "OrderByDescending";

                var result = Expression.Call(

                    typeof(Queryable), 
                    methodName,    
                    new Type[] { typeof(Problem), propertyType }, 
                    query.Expression, 
                    Expression.Quote(lambda)
                    );



                query = query.Provider.CreateQuery<Problem>(result);


            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);



            return query.ToListAsync();
        }
    }
}
