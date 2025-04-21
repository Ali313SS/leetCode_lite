using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Application.services;
namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemController : ControllerBase
    {

        private readonly IProblemService _problemService;        
        public ProblemController(IProblemService problemService)
        {
            
            _problemService = problemService;
        }
        [HttpGet("GetProblem")]
        public async Task<IActionResult> GetProblem([FromBody]ProblemDTO problem)
        {
           // var result = await _problemService.GetProblem(problemSource, problemId);
            //return Ok(result);
            return Ok();
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
