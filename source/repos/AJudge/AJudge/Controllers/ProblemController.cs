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

        [HttpGet("{problemId}")]
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

        [HttpPost("CSESProblem")]
        [Authorize]
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

        [HttpPost("Sumbit")]
        public async Task<IActionResult> SubmitProblem([FromBody] SumbitDTO sumbit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid data " });
            }
            try
            {
                int userId = 1; // Temporary hardcoded userId, replace with GetUserIdFromToken() if needed
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
        public string ProblemLink { get; set; }
        public string Code { get; set; }
    }
}