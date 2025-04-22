using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.RepoContracts
{
    public interface IProblemRepository:IBaseRepository<Problem>
    {
        Task<List<Problem>>? GetAllInPage(string sortBy,bool isAssending=true,int pageNumber=1,int pageSize=100);
    }
}
