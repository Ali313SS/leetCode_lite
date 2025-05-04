using AJudge.Application.DtO.SubmessionDTO;
using AJudge.Application.DTO.UserDTOS;
using AJudge.Application.services;
using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using System.Security.Claims;
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
            _unitOfWork = unitOfWork;
            _submissionService = submissionService;
        }





        [HttpGet("public-groups/{onlineJudge?}")]
        public async Task<IActionResult> GetAllSubmissionInPublicGroup([FromRoute] string? onlineJudge = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            SubmissionPagination submessions = await _submissionService.GetAllSubmissionInJudgePage(nameof(AJudge.Domain.Entities.Problem.ProblemSource),
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




        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMySubmissions([FromQuery] string? onlineJudge = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            // Get the authenticated user's ID from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            // Get submissions for the authenticated user
            var submissions = await _submissionService.GetUserSubmissionsAsync(userId, onlineJudge, pageNumber, pageSize);

            var response = new
            {
                ItemsResponse = submissions.Items.Select(x => SubmissopnResponseDTO.ConvertToSubmessionResponse(x)).ToList(),
                pagenumber = submissions.PageNumber,
                totalPages = submissions.TotalPages,
                hasPrevious = submissions.HasPrevious,
                hasNext = submissions.HasNext
            };

            return Ok(response);
        }



        [Authorize]
        [HttpGet("followed")]
        public async Task<IActionResult> GetFollowedUsersSubmissions([FromQuery] string? groupType = null, [FromQuery] string? onlineJudge = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            // Get the authenticated user's ID from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            // Validate groupType
            if (groupType != null && groupType != "Public" && groupType != "Private")
            {
                return BadRequest("Invalid groupType. Must be 'Public' or 'Private'.");
            }

            // Get submissions for followed users
            var submissions = await _submissionService.GetFollowedUsersSubmissionsAsync(userId, groupType, onlineJudge, pageNumber, pageSize);

            var response = new
            {
                ItemsResponse = submissions.Items.Select(x => SubmissopnResponseDTO.ConvertToSubmessionResponse(x)).ToList(),
                pagenumber = submissions.PageNumber,
                totalPages = submissions.TotalPages,
                hasPrevious = submissions.HasPrevious,
                hasNext = submissions.HasNext
            };

            return Ok(response);
        }





    }
}
