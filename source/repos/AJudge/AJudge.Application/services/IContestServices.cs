using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Application.DtO.ContestDTO;
using AJudge.Application.DtO.ProblemsDTO;
using AJudge.Application.DTO.GroupDTO;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Domain.Entities;

namespace AJudge.Application.services
{
    public interface IContestServices
    {
        Task<string> GetContestByIdAsync( int id);
        Task<ContestDtoRequest> AddContestAsync(ContestDto contest);

        Task<List<ProblemDTO>> GetProblemNameAndLinkByContestIdAsync(int contestId);
        Task<Contest> UpdateContestAsync(int id, UpdateContestRequest contestData);
      //  Task<Group> GetGroupByIdAsync(int id);
        Task<List<Contest>> GetContestsByGroupIdAsync(int groupId);
        Task<bool> AddContestToGroupAsync(int contestId, int groupId);
        Task<bool> RemoveContestFromGroupAsync(int contestId, int groupId);
        Task<ContestPagination> GetAllContestInPage(string sortBy, bool isAsinding = true, int pageNumber = 1, int pageSize = 100);

    }
}
