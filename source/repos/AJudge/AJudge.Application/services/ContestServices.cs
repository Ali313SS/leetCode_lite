using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Runtime.InteropServices;
using AJudge.Application.DtO.ContestDTO;
using AJudge.Application.DTO.GroupDTO;
using CContest = AJudge.Application.DtO.ContestDTO.CProblems;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection;
using AJudge.Infrastructure.Repositories;

namespace AJudge.Application.services
{
    public class ContestServices : IContestServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public ContestServices(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }


        public async Task<ContestDtoRequest> AddContestAsync(ContestDto contest)
        {
            var newContest = new Contest
            {
                Name = contest.Name,
                BeginTime = contest.BeginTime,
                EndTime = contest.EndTime,
                Tutorial = contest.Tutorial,
                Status = contest.Status,
                CreatorUserId = contest.CreatorUserId,
                GroupId = contest.GroupId,

            };
            await _context.Contests.AddAsync(newContest);
            await _context.SaveChangesAsync();
            var Contestreturn = new ContestDtoRequest
            {
                ContestId = newContest.ContestId,
                Name = newContest.Name,
                BeginTime = newContest.BeginTime,
                EndTime = newContest.EndTime,
                Tutorial = newContest.Tutorial,
                Status = newContest.Status,
                GroupId = newContest.GroupId,
                Problems = newContest.Problems.Select(c => new CProblems
                {
                    Id = c.ProblemId,
                    Name = c.ProblemName,

                }).ToList(),
            };
            return await Task.FromResult(Contestreturn);

        }

        public async Task<string> GetContestByIdAsync(int id)
        {
            var contest = await _context.Contests
        .Where(c => c.ContestId == id)
        .Select(c => c.Name)// Only select the Name property
        .FirstOrDefaultAsync();

            return contest;
        }

        public async Task<List<ProblemDTO>> GetProblemNameAndLinkByContestIdAsync(int contestId)
        {
            return await _context.Problems
                .Where(p => p.ContestId == contestId).Select(p => new ProblemDTO
                {
                    ProblemName = p.ProblemName,
                    problemLink = p.ProblemLink
                })
        .ToListAsync();
        }

        public async Task<Contest> UpdateContestAsync(int id, UpdateContestRequest contestData)
        {
            var contest = await _context.Contests
                .Include(c => c.Problems)
                .FirstOrDefaultAsync(c => c.ContestId == id);

            if (contest == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(contestData.Name))
            {
                contest.Name = contestData.Name;
            }

            if (contestData.BeginTime.HasValue)
            {
                contest.BeginTime = contestData.BeginTime.Value;
            }

            if (contestData.EndTime.HasValue)
            {
                contest.EndTime = contestData.EndTime.Value;
            }

            if (contestData.Problems != null && contestData.Problems.Count > 0)
            {
                _context.Problems.RemoveRange(contest.Problems);

                foreach (var problem in contestData.Problems)
                {
                    problem.ContestId = contest.ContestId;
                    _context.Problems.Add(problem);
                }
            }

            await _context.SaveChangesAsync();
            return contest;
        }



        // public async Task<Group> GetGroupByIdAsync(int id)
        //{
        //     return await _context.Groups
        //         .FirstOrDefaultAsync(g => g.GroupId == id);
        // }

        public async Task<List<Contest>> GetContestsByGroupIdAsync(int groupId)
        {
            //return await _context.ContestGroupMemberships
            //    .Where(cgm => cgm.GroupId == groupId)
            //    .Include(cgm => cgm.Contest)
            //    .ThenInclude(c => c.Problems)
            //    .Select(cgm => cgm.Contest)
            //    .ToListAsync();
            return await _context.Contests
                .Where(c => c.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<bool> AddContestToGroupAsync(int contestId, int groupId)
        {
            var contest = await _context.Contests.FindAsync(contestId);
            var group = await _context.Groups.FindAsync(groupId);

            if (contest == null || group == null)
            {
                return false;
            }

            //var existingMembership = await _context.ContestGroupMemberships
            //    .FirstOrDefaultAsync(cgm => cgm.ContestId == contestId && cgm.GroupId == groupId);

            //if (existingMembership == null)
            //{
            //    var membership = new ContestGroupMembership
            //    {
            //        ContestId = contestId,
            //        GroupId = groupId
            //    };

            //    _context.ContestGroupMemberships.Add(membership);
            //    await _context.SaveChangesAsync();
            //}

            return true;
        }

        public async Task<bool> RemoveContestFromGroupAsync(int contestId, int groupId)
        {
            //var membership = await _context.ContestGroupMemberships
            //    .FirstOrDefaultAsync(cgm => cgm.ContestId == contestId && cgm.GroupId == groupId);

            //if (membership == null)
            //{
            //    return false;
            //}

            //_context.ContestGroupMemberships.Remove(membership);
            //await _context.SaveChangesAsync();
            return true;
        }


        public async Task<ContestPagination> GetAllContestInPage(string sortBy, bool isAsinding = true, int pageNumber = 1, int pageSize = 100)
        {
            IQueryable<Contest> query = _unitOfWork.Contest.GetQuery();
            query = BuildSort(query, sortBy, isAsinding);
            ContestPagination contestPage = await ContestPagination.GetPageDetail(query, pageNumber, pageSize);

            return contestPage;

        }
        private IQueryable<Contest> BuildSort(IQueryable<Contest> query, string? sortBy, bool isAssending = true)
        {
            if (!string.IsNullOrEmpty(sortBy))
            {
                var parameter = Expression.Parameter(typeof(Contest), "x");
                var property = Expression.Property(parameter, sortBy);
                var propertyType = property.Type;
                var lambda = Expression.Lambda(property, parameter);
                string methodName = isAssending ? "OrderBy" : "OrderByDescending";
                var result = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { typeof(Contest), propertyType },
                    query.Expression,
                    Expression.Quote(lambda)
                    );
                query = query.Provider.CreateQuery<Contest>(result);
            }
            return query;
        }
    }
    public class ContestPagination
    {
        public List<Contest> Items { get; private set; }
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
        public ContestPagination(List<Contest> items, int count, int pageNumber, int pageSize)
        {
            Items = items ?? new List<Contest>();
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
        public static async Task<ContestPagination> GetPageDetail(IQueryable<Contest> source, int pageNumber = 1, int pageSize = 20)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new ContestPagination(items, count, pageNumber, pageSize);
        }

    
    }
}
