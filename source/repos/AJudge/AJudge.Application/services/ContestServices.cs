using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Application.DTO.GroupDTO;
using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AJudge.Application.services
{
    public class ContestServices : IContestServices
    {
        private readonly ApplicationDbContext _context;

        public ContestServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contest>> GetAllContestsAsync()
        {
            return await _context.Contests
                .Include(c => c.Problems)
                .ToListAsync();
        }

        public async Task<Contest> GetContestByIdAsync(int id)
        {
            return await _context.Contests
                .Include(c => c.Problems)
                .FirstOrDefaultAsync(c => c.ContestId == id);
        }

        public async Task<List<Problem>> GetProblemsByContestIdAsync(int contestId)
        {
            return await _context.Problems
                .Where(p => p.ContestId == contestId)
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

        

        public async Task<Group> GetGroupByIdAsync(int id)
        {
            return await _context.Groups
                .FirstOrDefaultAsync(g => g.GroupId == id);
        }

        public async Task<List<Contest>> GetContestsByGroupIdAsync(int groupId)
        {
            return await _context.ContestGroupMemberships
                .Where(cgm => cgm.GroupId == groupId)
                .Include(cgm => cgm.Contest)
                .ThenInclude(c => c.Problems)
                .Select(cgm => cgm.Contest)
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

            var existingMembership = await _context.ContestGroupMemberships
                .FirstOrDefaultAsync(cgm => cgm.ContestId == contestId && cgm.GroupId == groupId);

            if (existingMembership == null)
            {
                var membership = new ContestGroupMembership
                {
                    ContestId = contestId,
                    GroupId = groupId
                };

                _context.ContestGroupMemberships.Add(membership);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> RemoveContestFromGroupAsync(int contestId, int groupId)
        {
            var membership = await _context.ContestGroupMemberships
                .FirstOrDefaultAsync(cgm => cgm.ContestId == contestId && cgm.GroupId == groupId);

            if (membership == null)
            {
                return false;
            }

            _context.ContestGroupMemberships.Remove(membership);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
