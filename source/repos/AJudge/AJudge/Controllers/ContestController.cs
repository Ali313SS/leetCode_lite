
using System.Globalization;
using System.Security.Claims;
using AJudge.Application.DtO.ContestDTO;
using AJudge.Application.DtO.ProblemsDTO;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Application.DTO.UserDTOS;
using AJudge.Application.services;
using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
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
        private readonly IUnitOfWork _unitOfWork;

        public ContestController(IContestServices ContestServices, IUnitOfWork unitOfWork)
        {
            _ContestServices = ContestServices;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("CreateContest")]
        [Authorize]
        public async Task<ActionResult<ContestDtoRequest>> AddContest([FromBody] ContestDto contest)
        {
            if (contest == null)
                return BadRequest("Cntest data is null.");
            var userId = GetUserIdFromToken();
            contest.CreatorUserId = userId;
            var result = await _ContestServices.AddContestAsync(contest);
            return result;
            return BadRequest("Contest creation failed.");
        }


        /// <summary>
        /// Retrieves a paginated list of contests, optionally sorted by a specified property.
        /// </summary>
        /// <param name="sortBy">The property name to sort by (e.g., "Title", "StartDate").</param>
        /// <param name="isAssending">True for ascending sort, false for descending.</param>
        /// <param name="pageNumber">The page number to retrieve (starting from 1).</param>
        /// <param name="pageSize">The number of contests per page.</param>
        /// <returns>
        /// 200 OK with the paginated list of contests;  
        /// 400 Bad Request if sortBy is invalid;  
        /// 404 Not Found if the page does not exist.
        /// </returns>
        /// <response code="200">Returns the list of contests for the requested page.</response>
        /// <response code="400">Invalid sort property.</response>
        /// <response code="404">Page not found.</response>
       

        /// <summary>
        /// Retrieves the details of a specific contest by its ID.
        /// </summary>
        /// <param name="id">The ID of the contest to retrieve.</param>
        /// <returns>
        /// 200 OK with the contest details;  
        /// 404 Not Found if no contest exists with the given ID.
        /// </returns>
        /// <response code="200">Returns the contest details.</response>
        /// <response code="404">Contest not found.</response>
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

        /// <summary>
        /// Retrieves the list of problems associated with a specific contest.
        /// </summary>
        /// <param name="id">The ID of the contest.</param>
        /// <returns>
        /// A list of problems for the contest, including their names and links.
        /// </returns>
        /// <response code="200">Returns the list of contest problems.</response>
        /// <response code="404">Contest not found.</response>
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

        /// <summary>
        /// Updates an existing contest with new data.
        /// </summary>
        /// <param name="id">The ID of the contest to update.</param>
        /// <param name="contestData">The updated contest data.</param>
        /// <returns>
        /// The updated contest object if found and updated successfully.
        /// </returns>
        /// <response code="200">Returns the updated contest.</response>
        /// <response code="404">Contest with the specified ID was not found.</response>
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

        [HttpGet("cName/{cName}")]
        public async Task<IActionResult> GetAllContestInClub(string cName, bool isAssending = false, int pageNNumber = 1, int pageSSize = 20)
        {
            if (string.IsNullOrWhiteSpace(cName))
            {
                return BadRequest("Club name is required.");
            }
            //var s = "null";
            Contest? clubExist = await _unitOfWork.Contest.GetSpecific(x => x.Name == cName, null);
            if (clubExist == null)
                return NoContent();
            

            ContestPagination contests = await _ContestServices.GetAllContestInClubPerPage(nameof(AJudge.Domain.Entities.Contest.Name),
                 cName, nameof(AJudge.Domain.Entities.Contest.Name), isAssending, pageNNumber, pageSSize);
            if (pageNNumber < 1 || pageSSize < 1)
            {
                return BadRequest("Invalid pagination parameters.");
            }
            var response = new
            {
                ItemsResponse = contests.Items.Select(x => ContestResponseDTO.ConvertToContestResponse(x)).ToList(),
                pagenumber = contests.PageNumber,
                totalPages = contests.TotalPages,
                hasPrevious = contests.HasPrevious,
                hasNext = contests.HasNext
            };

            return Ok(response);
        }

        /// <summary>
        /// Retrieves all contests associated with a specific group.
        /// </summary>
        /// <param name="id">The ID of the group to retrieve contests for.</param>
        /// <returns>A list of contests belonging to the specified group.</returns>
        /// <response code="200">Returns the list of contests.</response>
        /// <response code="404">Group not found.</response>
        //[HttpGet("{id}/ContestByGroupId")]
        //[Authorize]

        //public async Task<ActionResult<IEnumerable<Contest>>> GetContestsByGroupId(int id)
        //{
        //var group = await _ContestServices.GetGroupByIdAsync(id);
        // if (group == null)
        //{
        //    return NotFound();
        //  }
        //  return await _ContestServices.GetContestsByGroupIdAsync(id);
        //}


        /// <summary>
        /// Adds a contest to a specified group.
        /// </summary>
        /// <param name="request">Contains the IDs of the contest and the group.</param>
        /// <returns>Returns 200 OK if the contest is successfully added to the group; otherwise, returns 400 Bad Request.</returns>
        /// <response code="200">Contest was successfully added to the group.</response>
        /// <response code="400">Failed to add contest to group due to invalid contest or group ID.</response>
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


        /// <summary>
        /// Removes a contest from a specified group.
        /// </summary>
        /// <param name="request">The request containing the ContestId and GroupId.</param>
        /// <returns>
        /// Returns <see cref="OkResult"/> if the contest was successfully removed from the group.
        /// Returns <see cref="BadRequestObjectResult"/> if the removal failed, 
        /// which may indicate that the group doesn't exist or the contest is not in the group.
        /// </returns>
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
        private int GetUserIdFromToken()
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            throw new UnauthorizedAccessException("User ID not found in token.");
        }
    }
}
