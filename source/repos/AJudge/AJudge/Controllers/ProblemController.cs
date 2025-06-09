using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Application.services;
using AJudge.Domain.RepoContracts;
using AJudge.Domain.Entities;
using AJudge.Application.DtO.ProblemsDTO;
using AJudge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AJudge.Application.DTO.GroupDTO;
using System;
using System.Security.Claims;
using static AJudge.Domain.Entities.Problem;

namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProblemService _problemService;
        private readonly IGroupServices _groupServices;
        private readonly ApplicationDbContext _context;

        public ProblemController(ApplicationDbContext context, IProblemService problemService, IGroupServices groupServices)
        {
            _context = context;
            _problemService = problemService;
            _groupServices = groupServices;
        }

        /// <summary>
        /// Retrieves detailed information about a specific problem.
        /// </summary>
        /// <param name="problemId">The unique identifier of the problem.</param>
        /// <returns>
        /// Returns the detailed problem information if found; otherwise, returns a 404 Not Found response.
        /// If the user is authenticated, their user ID is used to tailor the response (e.g., submission state).
        /// </returns>
        [HttpGet("{problemId}")]
        [ProducesResponseType(typeof(ProblemDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProblemDetails(int problemId)
        {
            int? userId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim?.Value, out int parsedId))
                {
                    userId = parsedId;
                }
            }

            ProblemDetailsDTO? problemDetailsDTO = await _problemService.GetProblemDetailsAsync(problemId, userId);
            if (problemDetailsDTO == null)
                return NotFound(new { message = "No such problem found." });

            return Ok(problemDetailsDTO);
        }
        /// <summary>
        /// Fetches a problem from an external source (like CSES), adds it to the contest, and returns detailed problem info.
        /// </summary>
        /// <param name="problemDto">Problem details including source, link, problem ID, and contest ID.</param>
        /// <returns>Returns detailed problem info on success or an error message.</returns>
        [HttpPost("CSESProblem")]
        [Authorize]
        [ProducesResponseType(typeof(ProblemDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FetchP([FromBody] FetchProblemDto problemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid data " });
            }
            try
            {
                var Contest = _context.Contests.FirstOrDefault(x => x.ContestId == problemDto.ContestID);
                int userId = GetUserIdFromToken();
                if (Contest == null)
                {
                    return BadRequest(new { message = "invalid ContestId" });
                }
                int groupID = Contest.GroupId;
                if (!_groupServices.UserManagerInGroup(groupID, userId).Result)
                {
                    return BadRequest(new { message = "You are not a manager in this group" });
                }
                var problem = await _problemService.FetchProblem(problemDto);
                if (problem == null)
                {
                    return BadRequest(new { message = "problem not added" });
                }
                Problem NewProblem = new Problem
                {
                    ProblemName = problem.ProblemName,
                    ProblemSource = problemDto.ProblemSource,
                    Description = problem.Description,
                    InputFormat = problem.InputFormat,
                    OutputFormat = problem.OutputFormat,
                    ProblemLink = problem.ProblemLink,
                    Rating = problem.Rating,
                    ContestId = problemDto.ContestID,
                    numberOfTestCases = problem.numberOfTestCases,
                    ProblemSourceID = problem.ProblemSourceID,
                    TestCases = problem.TestCases.Select(tc => new TestCase
                    {
                        Input = tc.Input,
                        Output = tc.Output
                    }).ToList(),
                };
                ProblemDetailsDTO problemDTO = ProblemDetailsDTO.ConvertToProblemDetalsDTO(
                    NewProblem,
                    "Not Submitted",
                    new List<string>(),
                   problem.TestCases.Select(tc => new InputOutputTestCasesDTO
                   {
                       Input = tc.Input,
                       Output = tc.Output
                   }).ToList()
                );
                try
                {
                    Console.Beep();
                    Console.Beep();
                    await _context.Problems.AddAsync(NewProblem);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "error occure", error = ex.Message });
                }
                return Ok(problemDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "error occure", error = ex.Message });
            }
        }
        /// <summary>
        /// Submit a solution code for a problem.
        /// </summary>
        /// <param name="submit">Contains problem link and submitted code.</param>
        /// <returns>Returns result of the submission or error.</returns>
        [HttpPost("Sumbit")]
        [Authorize]
        public async Task<IActionResult> SubmitProblem([FromBody] SumbitDTO sumbit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid data " });
            }
            try
            {
                int userId = GetUserIdFromToken(); // Temporary hardcoded userId, replace with GetUserIdFromToken() if needed
                var result = await _problemService.SumbitProblem(sumbit.ProblemLink, userId, sumbit.Code);
                if (result == null)
                {
                    return BadRequest(new { message = "problem not added" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "error occure", error = ex.Message });
            }
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

    public class SumbitDTO
    {
        /// <summary>
        /// link of problem
        /// </summary>
        public string ProblemLink { get; set; }
        /// <summary>
        /// content of code submission
        /// </summary>
        public string Code { get; set; }
    }
}