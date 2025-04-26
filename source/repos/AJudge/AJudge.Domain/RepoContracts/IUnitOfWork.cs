using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.RepoContracts
{
    public interface IUnitOfWork:IDisposable
    {
         IBaseRepository<Problem> Problem { get;  }
        Task CompleteAsync();
    }
}
