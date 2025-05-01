
using System.Security.Claims;
using AJudge.Application.DtO.ContestDTO;
using AJudge.Application.DtO.ProblemsDTO;
using AJudge.Application.DTO.ProblemsDTO;
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
        
        [HttpGet("Contest")]
        public async Task<IActionResult> GetAllContestPerSpecificPage(string? sortBy, bool isAssending = true, int pageNumber = 1, int pageSize = 20)
        {

            var sortByy = typeof(Contest).GetProperty(sortBy);
            if (sortByy == null)
            {
                return BadRequest("No such property");
            }
            var page = await _ContestServices.GetAllContestInPage(sortBy, isAssending, pageNumber, pageSize);


            if (page == null)
                return NotFound("No such Page");

            var DisPage = ContestPageDto.ConvertToContestDto(page);


            return Ok(DisPage);
        }


        [HttpGet("{id}/Getcontest")]
        [Authorize]

        public async Task<ActionResult> GetContestById(int id)
        {
            var contest = await _ContestServices.GetContestByIdAsync(id);
            if (contest == null)
            {
                return NotFound();
            }
          
            return Ok(new { Name = contest });
        }
        
        [HttpGet("{id}/Contestproblems")]
        [Authorize]

        public async Task<ActionResult<IEnumerable<ProblemDTO>>> GetContestProblems(int id)
        {
            var contest = await _ContestServices.GetContestByIdAsync(id);
            if (contest == null)
            {
                return NotFound();
            }

            var problemDetails = await _ContestServices.GetProblemNameAndLinkByContestIdAsync(id);
            return Ok(problemDetails);
        }


        [HttpPut("{id}/updatecontest")]
        [Authorize]

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
        [Authorize]

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
        [Authorize]

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
        [Authorize]

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
