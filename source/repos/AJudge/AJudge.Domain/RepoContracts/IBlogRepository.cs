using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.RepoContracts
{
    public interface IBlogRepository:IBaseRepository<Blog>
    {
        Task<IEnumerable<Blog>?> GetAllBlogs(string[] includes);
        Task<Blog?> GetBlogById(int id, string[] includes);
    }
}
