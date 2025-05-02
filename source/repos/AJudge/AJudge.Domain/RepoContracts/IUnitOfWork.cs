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
         IBaseRepository<Group> Group { get;  }
         IBaseRepository<Submission> Submission { get; }
         IBaseRepository<User> User { get; }

         IBaseRepository<Vote> Vote { get; }

        IBaseRepository<Comment> Comment { get;  }
         IBlogRepository Blog { get;  }
        Task CompleteAsync();
        void Attach<T>(T item);
        void MarkModified<T>(T item, string[] propertyNames);
    }
}
