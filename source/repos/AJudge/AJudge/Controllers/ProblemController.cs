using Microsoft.AspNetCore.Mvc;
using AJudge.Application.DtO.ProblemsDTO;
using AJudge.Application.services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using AJudge.Application.DTO.ProblemsDTO;

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

        [HttpGet("{problemName}")]
        public async Task<IActionResult> GetProblem(string problemName)
        {
            var problemDetailsDTO = await _problemService.GetProblemByName(problemName);
            if (problemDetailsDTO == null)
            {
                return NotFound("لا يوجد مشكلة بهذا الاسم");
            }
            return Ok(problemDetailsDTO);
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetAllProblemsByPerSpecificPage(
            string? sortBy,
            bool isAssending = true,
            int pageNumber = 1,
            int pageSize = 100)
        {
            var problemsDTO = await _problemService.GetAllProblems(sortBy, isAssending, pageNumber, pageSize);
            if (problemsDTO == null || !problemsDTO.Any())
            {
                return NotFound("لا توجد مشاكل متاحة");
            }
            return Ok(problemsDTO);
        }

        [HttpPost("CreateProblem")]
        [AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProblem([FromBody] ProblemDTO problemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _problemService.AddProblem(problemDto);
                if (!result)
                {
                    return StatusCode(500, new { message = "خطأ أثناء إنشاء المشكلة" });
                }

                var createdProblem = await _problemService.GetProblemByName(problemDto.ProblemName);
                if (createdProblem == null)
                {
                    return StatusCode(500, new { message = "تم إنشاء المشكلة لكن فشل جلبها" });
                }

                return CreatedAtAction(nameof(GetProblem), new { problemName = createdProblem.ProblemName }, createdProblem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطأ أثناء إنشاء المشكلة", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProblem(int id, [FromBody] ProblemDTO problemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProblem = await _problemService.GetProblem(id);
            if (existingProblem == null)
            {
                return NotFound(new { message = $"المشكلة بالـ ID {id} مش موجوده" });
            }

            problemDto.ProblemId = id;
            var result = await _problemService.ChangeProblemStatement(problemDto);
            if (!result)
            {
                return StatusCode(500, new { message = "خطأ أثناء تحديث المشكلة" });
            }

            return Ok(await _problemService.GetProblem(id));
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProblem(int id)
        {
            var result = await _problemService.DeleteProblem(id);
            if (!result)
            {
                return NotFound(new { message = $"المشكلة بالـ ID {id} مش موجوده" });
            }

            return NoContent();
        }
    }
}