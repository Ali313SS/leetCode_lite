using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private  readonly ApplicationDbContext _context;
        public IBaseRepository<Problem> Problem { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Problem=new BaseRepoitory<Problem>(_context);   
        }

        public void Dispose()
        {
           _context.Dispose();
        }

        public async Task CompleteAsync()
        {
           await _context.SaveChangesAsync();
        }
         
    }
}
