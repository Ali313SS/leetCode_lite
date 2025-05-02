using AJudge.Application.DtO.SubmessionDTO;
using AJudge.Application.DTO.UserDTOS;
using AJudge.Application.services;
using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using System.Text.RegularExpressions;

namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubmissionService _submissionService;
        public SubmissionController(IUnitOfWork unitOfWork, ISubmissionService submissionService)
        {
            _unitOfWork= unitOfWork;
            _submissionService= submissionService;  
        }


   


        [HttpGet("public-groups/{onlineJudge?}")]
        public async Task<IActionResult> GetAllSubmissionInPublicGroup([FromRoute]string ?onlineJudge=null,[FromQuery]int pageNumber=1,[FromQuery]int pageSize=20)
        {
            SubmissionPagination submessions =await  _submissionService.GetAllSubmissionInJudgePage(nameof(AJudge.Domain.Entities.Problem.ProblemSource), 
                onlineJudge, pageNumber, pageSize);

            var response = new
            {
                ItemsResponse = submessions.Items.Select(x => SubmissopnResponseDTO.ConvertToSubmessionResponse(x)).ToList(),
                pagenumber = submessions.PageNumber,
                totalPages = submessions.TotalPages,
                hasPrevious = submessions.HasPrevious,
                hasNext = submessions.HasNext
            };
            return Ok(response);
        }
    }
}
