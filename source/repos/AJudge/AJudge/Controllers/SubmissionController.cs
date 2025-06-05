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




        /// <summary>
        /// Retrieves paginated submissions from all public groups, optionally filtered by online judge source.
        /// </summary>
        /// <param name="onlineJudge">
        /// Optional route parameter to filter submissions by the specified online judge source.
        /// If not provided, submissions from all online judges will be included.
        /// </param>
        /// <param name="pageNumber">
        /// Query parameter specifying the page number to retrieve. Defaults to 1.
        /// Must be greater than 0.
        /// </param>
        /// <param name="pageSize">
        /// Query parameter specifying the number of submissions per page. Defaults to 20.
        /// Must be greater than 0.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a paginated list of submissions with metadata:
        /// <list type="bullet">
        /// <item>ItemsResponse: List of submission DTOs</item>
        /// <item>pagenumber: Current page number</item>
        /// <item>totalPages: Total number of pages</item>
        /// <item>hasPrevious: Indicates if there is a previous page</item>
        /// <item>hasNext: Indicates if there is a next page</item>
        /// </list>
        /// </returns>
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



        /// <summary>
        /// Retrieves paginated submissions made by the authenticated user, optionally filtered by online judge source.
        /// </summary>
        /// <param name="onlineJudge">
        /// Optional query parameter to filter submissions by the specified online judge source.
        /// If not provided, submissions from all online judges will be included.
        /// </param>
        /// <param name="pageNumber">
        /// Query parameter specifying the page number to retrieve. Defaults to 1.
        /// Must be greater than 0.
        /// </param>
        /// <param name="pageSize">
        /// Query parameter specifying the number of submissions per page. Defaults to 20.
        /// Must be greater than 0.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a paginated list of the user's submissions with metadata:
        /// <list type="bullet">
        /// <item>ItemsResponse: List of submission DTOs</item>
        /// <item>pagenumber: Current page number</item>
        /// <item>totalPages: Total number of pages</item>
        /// <item>hasPrevious: Indicates if there is a previous page</item>
        /// <item>hasNext: Indicates if there is a next page</item>
        /// </list>
        /// Returns Unauthorized if the user ID from the token is invalid.
        /// </returns>
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


        /// <summary>
        /// Retrieves paginated submissions made by users followed by the authenticated user, with optional filtering.
        /// </summary>
        /// <param name="groupType">
        /// Optional query parameter to filter submissions by group privacy type.
        /// Allowed values are "Public" or "Private". If not provided, submissions from all groups are included.
        /// </param>
        /// <param name="onlineJudge">
        /// Optional query parameter to filter submissions by the specified online judge source.
        /// If not provided, submissions from all online judges will be included.
        /// </param>
        /// <param name="pageNumber">
        /// Query parameter specifying the page number to retrieve. Defaults to 1.
        /// Must be greater than 0.
        /// </param>
        /// <param name="pageSize">
        /// Query parameter specifying the number of submissions per page. Defaults to 20.
        /// Must be greater than 0.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> with paginated submissions of followed users and pagination metadata:
        /// <list type="bullet">
        /// <item>ItemsResponse: List of submission DTOs</item>
        /// <item>pagenumber: Current page number</item>
        /// <item>totalPages: Total number of pages</item>
        /// <item>hasPrevious: Indicates if there is a previous page</item>
        /// <item>hasNext: Indicates if there is a next page</item>
        /// </list>
        /// Returns Unauthorized if the user ID from the token is invalid.
        /// Returns BadRequest if the groupType parameter is invalid.
        /// </returns>
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
