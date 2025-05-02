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
        public IBaseRepository<Group> Group { get; private set; }
        public IBaseRepository<Submission> Submission { get; private set; }
        public IBaseRepository<User> User { get; private set; }
        public IBaseRepository<Vote> Vote { get; private set; }
        public IBaseRepository<Comment> Comment { get; private set; }
        public IBlogRepository Blog { get; private set; }
        public IBaseRepository<Contest> Contest { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Problem=new BaseRepoitory<Problem>(_context);
            Submission = new BaseRepoitory<Submission>(_context);
            Vote=new BaseRepoitory<Vote>(_context); 
            User=new BaseRepoitory<User>(_context); 
            Group=new BaseRepoitory<Group>(_context); 
            Blog=new BlogRepository(_context);
            Comment=new BaseRepoitory<Comment>(_context);
        }

        public void Dispose()
        {
           _context.Dispose();
        }

        public async Task CompleteAsync()
        {
           await _context.SaveChangesAsync();
        }

        public void Attach<T>(T item)
        {
              _context.Attach(item);
        }

        public void MarkModified<T>(T item, string[] propertyNames)
        {
            foreach(var prop in propertyNames)
            {
                _context.Entry(item).Property(prop).IsModified = true;
            }
        }
    }
}
