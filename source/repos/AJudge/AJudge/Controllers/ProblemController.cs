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
namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IProblemService _problemService;


        private readonly ApplicationDbContext _context;
        //public ProblemController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}
        //public ProblemController(IProblemService problemService)
        //{
        //    _problemService = problemService;
        //}

        public ProblemController(ApplicationDbContext context, IProblemService problemService)
        {
            _context = context;
            _problemService = problemService;
        }

        [HttpGet("{problemName}")]
        public async Task<IActionResult> GetProblem(string problemName)
        {
            ProblemDetailsDTO? problemDEtailsDTO = await _problemService.GetProblemByName(problemName);
            if (problemDEtailsDTO == null)
                return NotFound("No such Problem");
            return Ok(problemDEtailsDTO);
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetAllProblemsByPerSpecificPage(string? sortBy, bool isAssending = true, int pageNumber = 1, int pageSize = 100)
        {
            List<ProblemDTO> problemsDTO = await _problemService.GetAllProblems(sortBy, isAssending, pageNumber, pageSize);
            if (problemsDTO == null)
                return NotFound("No such Problem");

            return Ok(problemsDTO);
        }



        //[HttpPost("CreateProblem")]
        //public async Task<IActionResult> CreateProblem([FromBody] ProblemDTO problem)
        //{
        //    //var result = await _problemService.CreateProblem(problemDTO);
        //    //return Ok(result);
        //    return Ok();
        //}



        [HttpPost("CreateProblem")]
        public async Task<IActionResult> CreateProblem([FromBody] ProblemDTO problemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid data " });
            }

            try
            {
                var problem = new Problem
                {
                    ProblemSource = problemDto.ProblemSource,
                    ProblemLink = problemDto.problemLink,
                    ProblemName = problemDto.ProblemName,
                    Rating = problemDto.Rating,
                    
                    ProblemSourceID = "ProblemSourceID",
                    ContestId = problemDto.ContestId,
                    Description = "Description Placeholder",
                    InputFormat = "Input Format Placeholder",
                    OutputFormat = "Output Format Placeholder",
                    numberOfTestCases = problemDto.NumberOfTestCases
                };

                await _context.Problems.AddAsync(problem);
                await _context.SaveChangesAsync();

                var problemDetails = ProblemDetailsDTO.ConvertToProblemDetalsDTO(
                    problem,
                    "Not Submitted",
                    new List<string>(),
                    new InputOutputTestCasesDTO() 
                );

                return CreatedAtAction(nameof(GetProblem), new { problemName = problem.ProblemName }, new
                {
                    message = "problem added ",
                    problem = problemDetails
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "error occure", error = ex.Message });
            }
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProblem(int id, [FromBody] ProblemDTO problemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid data " });

            }

            try
            {
                var existingProblem = await _context.Problems
                    .FirstOrDefaultAsync(p => p.ProblemId == id);

                if (existingProblem == null)
                {
                    return NotFound(new { message = "invalid data " });

                }

                
                existingProblem.ProblemSource = problemDto.ProblemSource ?? existingProblem.ProblemSource;
                existingProblem.ProblemLink = problemDto.problemLink ?? existingProblem.ProblemLink;
                existingProblem.ProblemName = problemDto.ProblemName ?? existingProblem.ProblemName;
                existingProblem.Rating = problemDto.Rating > 0 ? problemDto.Rating : existingProblem.Rating;
                existingProblem.ContestId = problemDto.ContestId > 0 ? problemDto.ContestId : existingProblem.ContestId;
                existingProblem.numberOfTestCases = problemDto.NumberOfTestCases > 0 ? problemDto.NumberOfTestCases : existingProblem.numberOfTestCases;
                existingProblem.Description = "string...."; 
                existingProblem.InputFormat = "string....";
                existingProblem.OutputFormat ="string....";

                _context.Problems.Update(existingProblem);
                await _context.SaveChangesAsync();

                var problemDetails = ProblemDetailsDTO.ConvertToProblemDetalsDTO(
                    existingProblem,
                    "Not Submitted",
                    new List<string>(),
                    new InputOutputTestCasesDTO()
                );

                return Ok(new
                {
                    message = "problem updated ",

                    problem = problemDetails
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "error occure", error = ex.Message });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProblem(int id)
        {
            try
            {
                var problem = await _context.Problems
                    .FirstOrDefaultAsync(p => p.ProblemId == id);

                if (problem == null)
                {
                    return NotFound(new { message = "invalid data " });
                }

                _context.Problems.Remove(problem);
                await _context.SaveChangesAsync();

                return Ok(new { message = "problem deleted " });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "error occure ", error = ex.Message });
            }
        }

    }
}