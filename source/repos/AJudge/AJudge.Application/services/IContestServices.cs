using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Domain.Entities;

namespace AJudge.Application.services
{
    public interface IContestServices
    {
        Task<List<Contest>> GetAllContestsAsync();
        Task<Contest> GetContestByIdAsync(int id);
        Task<List<Problem>> GetProblemsByContestIdAsync(int contestId);
       Task<Contest> UpdateContestAsync(int id, UpdateContestRequest contestData);
        Task<Group> GetGroupByIdAsync(int id);
        Task<List<Contest>> GetContestsByGroupIdAsync(int groupId);
        Task<bool> AddContestToGroupAsync(int contestId, int groupId);
        Task<bool> RemoveContestFromGroupAsync(int contestId, int groupId);
    }
}
