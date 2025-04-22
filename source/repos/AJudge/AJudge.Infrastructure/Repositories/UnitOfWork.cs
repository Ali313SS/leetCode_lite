//using AJudge.Domain.Entities;
//using AJudge.Domain.RepoContracts;
//using AJudge.Infrastructure.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AJudge.Infrastructure.Repositories
//{
//    public class UnitOfWork : IUnitOfWork
//    {
//        private readonly ApplicationDbContext _context;
//        public IProblemRepository Problem { get; private set; }
//        public UnitOfWork(ApplicationDbContext context)
//        {
//            _context = context;
//            Problem = new ProblemRepository(_context);
//        }

//        public void Dispose()
//        {
//            _context.Dispose();
//        }

//        public async Task CompleteAsync()
//        {
//            await _context.SaveChangesAsync();
//        }

//    }
//}

//using AJudge.Domain.RepoContracts;
//using AJudge.Infrastructure.Data;
//using AJudge.Infrastructure.Repositories;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading.Tasks;

//namespace AJudge.Infrastructure
//{
//    public class UnitOfWork : IUnitOfWork
//    {
//        private readonly ApplicationDbContext _context;
//        private IProblemRepository _problemRepository;

//        public UnitOfWork(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public IProblemRepository Problem
//        {
//            get { return _problemRepository ??= new ProblemRepository(_context); }
//        }

//        public async Task<int> SaveChangesAsync()
//        {
//            return await _context.SaveChangesAsync();
//        }
//        public void Dispose()
//        {
//            _context.Dispose();
//        }

//        Task IUnitOfWork.SaveChangesAsync()
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public interface IUnitOfWork
//    {
//        IProblemRepository Problem { get; }
//        Task SaveChangesAsync();
//    }
//}

using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Data;
using AJudge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AJudge.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IProblemRepository _problem;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IProblemRepository Problem
        {
            get
            {
                _problem ??= new ProblemRepository(_context);
                return _problem;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}