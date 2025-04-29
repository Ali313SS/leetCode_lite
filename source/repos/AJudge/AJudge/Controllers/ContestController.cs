using AJudge.Application.services;
using AJudge.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ContestController : ControllerBase
    {


        private readonly IContestServices _ContestServices;
        public ContestController(IContestServices ContestServices)
        {
            _ContestServices = ContestServices;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contest>>> GetAllContests()
        {
            return await _ContestServices.GetAllContestsAsync();
        }

        [HttpGet("{id}/Getcontest")]
        public async Task<ActionResult<Contest>> GetContestById(int id)
        {
            var contest = await _ContestServices.GetContestByIdAsync(id);
            if (contest == null)
            {
                return NotFound();
            }
            return contest;
        }

        [HttpGet("{id}/Contestproblems")]
        public async Task<ActionResult<IEnumerable<Problem>>> GetContestProblems(int id)
        {
            var contest = await _ContestServices.GetContestByIdAsync(id);
            if (contest == null)
            {
                return NotFound();
            }
            return await _ContestServices.GetProblemsByContestIdAsync(id);
        }

        [HttpPut("{id}/updatecontest")]
        public async Task<ActionResult<Contest>> UpdateContest(int id, UpdateContestRequest contestData)
        {
            var updatedContest = await _ContestServices.UpdateContestAsync(id, contestData);
            if (updatedContest == null)
            {
                return NotFound();
            }
            return updatedContest;
        }

        

        [HttpGet("{id}/ContestByGroupId")]
        public async Task<ActionResult<IEnumerable<Contest>>> GetContestsByGroupId(int id)
        {
            var group = await _ContestServices.GetGroupByIdAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            return await _ContestServices.GetContestsByGroupIdAsync(id);
        }

        [HttpPost("AddContestTOGroup")]
        public async Task<ActionResult> AddContestToGroup(ContestGroupRequest request)
        {
            var success = await _ContestServices.AddContestToGroupAsync(request.ContestId, request.GroupId);
            if (!success)
            {
                return BadRequest("Failed to add contest to group. Either contest or group doesn't exist.");
            }
            return Ok();
        }

        [HttpPost("removeContestFromGroup")]
        public async Task<ActionResult> RemoveContestFromGroup(ContestGroupRequest request)
        {
            var success = await _ContestServices.RemoveContestFromGroupAsync(request.ContestId, request.GroupId);
            if (!success)
            {
                return BadRequest("Failed to remove contest from group. Either group doesn't exist or contest is not in group.");
            }
            return Ok();
        }
    }
}
