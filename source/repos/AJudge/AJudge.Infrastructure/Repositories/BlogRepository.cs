using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Infrastructure.Repositories
{
    public class BlogRepository : BaseRepoitory<Blog>, IBlogRepository
    {
        public BlogRepository(ApplicationDbContext context):base(context)
        {
            
        }
        public async Task<IEnumerable<Blog>?> GetAllBlogs(string[] includes)
        {
            IQueryable<Blog> query = _context.Set<Blog>();
            foreach (var include in includes)
            {
                var propertyName = typeof(Blog).GetProperty(include);
                if (propertyName != null)
                    query = query.Include(include);
                else
                    throw new InvalidOperationException($"Property '{include}' does not exist on type '{typeof(Blog)}'.");
            }
            query = query.OrderByDescending(x => x.CreatedAt);
            return await query.ToListAsync();
        }

        public async Task<Blog?> GetBlogById(int id, string[] includes)
        {

            IQueryable<Blog> query = _context.Set<Blog>();

            foreach (var include in includes)
            {
                var propertyInfo = typeof(Blog).GetProperty(include);
                if (propertyInfo != null)
                {
                    query = query.Include(include);
                }
                else
                {
                    throw new InvalidOperationException($"Property '{include}' does not exist on type '{typeof(Blog)}'.");
                }
            }
                return await query.FirstOrDefaultAsync(x=>x.BlogId== id);
        }


       
      
    }
}
