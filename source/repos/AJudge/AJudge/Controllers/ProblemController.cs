using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Application.services;
using AJudge.Domain.RepoContracts;
using AJudge.Domain.Entities;
using AJudge.Application.DtO.ProblemsDTO;
using AJudge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;  

        private readonly IProblemService _problemService;        
        

        private readonly ApplicationDbContext _context;        
        public ProblemController(IProblemService problemService)
        {
            _problemService = problemService;
        }
           
        [HttpGet("{problemName}")]
        public async Task<IActionResult> GetProblem(string problemName)
        {
            ProblemDetailsDTO? problemDEtailsDTO = await  _problemService.GetProblemByName(problemName);
            if (problemDEtailsDTO == null)
                return NotFound("No such Problem");
            return Ok(problemDEtailsDTO);
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetAllProblemsByPerSpecificPage(string? sortBy,bool isAssending=true,int pageNumber=1,int pageSize=100)
        {
            List<ProblemDTO> problemsDTO = await _problemService.GetAllProblems(sortBy, isAssending, pageNumber, pageSize);
            if (problemsDTO == null)
                return NotFound("No such Problem");

            return Ok(problemsDTO);
        }



        [HttpPost("CreateProblem")]
        public async Task<IActionResult> CreateProblem([FromBody] ProblemDTO problem)
        {
            //var result = await _problemService.CreateProblem(problemDTO);
            //return Ok(result);
            return Ok();
        }




            
          








        

    }
}
