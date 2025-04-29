<<<<<<< HEAD
﻿using AJudge.Application.services;
using AJudge.Domain.Entities;
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
        public ContestController(IContestServices ContestServices)
        {
            _ContestServices = ContestServices;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contest>>> GetAllContests()
        {
            return await _ContestServices.GetAllContestsAsync();
        }

        [HttpGet("{id}/Getcontest")]
        public async Task<ActionResult<Contest>> GetContestById(int id)
        {
            var contest = await _ContestServices.GetContestByIdAsync(id);
            if (contest == null)
            {
                return NotFound();
            }
            return contest;
        }

        [HttpGet("{id}/Contestproblems")]
        public async Task<ActionResult<IEnumerable<Problem>>> GetContestProblems(int id)
        {
            var contest = await _ContestServices.GetContestByIdAsync(id);
            if (contest == null)
            {
                return NotFound();
            }
            return await _ContestServices.GetProblemsByContestIdAsync(id);
        }

        [HttpPut("{id}/updatecontest")]
        public async Task<ActionResult<Contest>> UpdateContest(int id, UpdateContestRequest contestData)
        {
            var updatedContest = await _ContestServices.UpdateContestAsync(id, contestData);
            if (updatedContest == null)
            {
                return NotFound();
            }
            return updatedContest;
        }

        

        [HttpGet("{id}/ContestByGroupId")]
        public async Task<ActionResult<IEnumerable<Contest>>> GetContestsByGroupId(int id)
        {
            var group = await _ContestServices.GetGroupByIdAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            return await _ContestServices.GetContestsByGroupIdAsync(id);
        }

        [HttpPost("AddContestTOGroup")]
        public async Task<ActionResult> AddContestToGroup(ContestGroupRequest request)
        {
            var success = await _ContestServices.AddContestToGroupAsync(request.ContestId, request.GroupId);
            if (!success)
            {
                return BadRequest("Failed to add contest to group. Either contest or group doesn't exist.");
            }
            return Ok();
        }

        [HttpPost("removeContestFromGroup")]
        public async Task<ActionResult> RemoveContestFromGroup(ContestGroupRequest request)
        {
            var success = await _ContestServices.RemoveContestFromGroupAsync(request.ContestId, request.GroupId);
            if (!success)
            {
                return BadRequest("Failed to remove contest from group. Either group doesn't exist or contest is not in group.");
            }
            return Ok();
        }
    }
}
=======
﻿using AJudge.Application.DtO;
using AJudge.Application.DtO.AnnouncementsDTO;
using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AJudge.API.Controllers
{
    [Route("api/contests")]
    [ApiController]
    public class ContestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/contests/{contestId}/problems
        [HttpGet("{contestId}/problems")]
        public async Task<ActionResult> GetProblemsInContest(int contestId)
        {
            var contest = await _context.Contests
                .Include(c => c.Problems)
                .FirstOrDefaultAsync(c => c.ContestId == contestId);

            if (contest == null)
            {
                return NotFound("Contest not found.");
            }

            if (contest.Problems == null || !contest.Problems.Any())
            {
                return Ok(new { Message = "No problems found for this contest." });
            }

            return Ok(contest.Problems);
        }

        // GET: /api/contests/{contestId}/submissions
        [HttpGet("{contestId}/submissions")]
        public async Task<ActionResult> GetSubmissionsInContest(int contestId)
        {
            var contest = await _context.Contests
                .FirstOrDefaultAsync(c => c.ContestId == contestId);

            if (contest == null)
            {
                return NotFound("Contest not found.");
            }

            var submissions = await _context.Submission
                .Include(s => s.User)
                .Include(s => s.Problem)
                .Where(s => s.Problem.ContestId == contestId)
                .ToListAsync();

            if (!submissions.Any())
            {
                return Ok(new { Message = "No submissions found for this contest." });
            }

            return Ok(submissions);
        }

        // POST: /api/contests/{contestId}/announcements
        [Authorize]
        [HttpPost("{contestId}/announcements")]
        public async Task<ActionResult<CreateAnnouncementResponse>> CreateAnnouncement(int contestId, [FromBody] string message)
        {
            var contest = await _context.Contests
                .FirstOrDefaultAsync(c => c.ContestId == contestId);

            if (contest == null)
            {
                return NotFound("Contest not found.");
            }

            // Check if the user is the creator
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            if (contest.CreatorUserId != userId)
            {
                return Forbid("Only the contest creator can post announcements.");
            }

            var announcement = new Announcement
            {
                ContestId = contestId,
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            // Map to DTO
            var announcementDto = new AnnouncementsDTO
            {
                AnnouncementId = announcement.AnnouncementId,
                ContestId = announcement.ContestId,
                UserId = announcement.UserId,
                Message = announcement.Message,
                CreatedAt = announcement.CreatedAt
            };

            // Create response with message
            var response = new CreateAnnouncementResponse
            {
                Message = "Announcement added successfully",
                Announcement = announcementDto
            };

            return CreatedAtAction(nameof(GetAnnouncement), new { id = announcement.AnnouncementId }, response);
        }

        // GET: /api/contests/announcements/{id}
        [HttpGet("announcements/{id}")]
        public async Task<ActionResult<AnnouncementsDTO>> GetAnnouncement(int id)
        {
            var announcement = await _context.Announcements
                .FirstOrDefaultAsync(a => a.AnnouncementId == id);

            if (announcement == null)
            {
                return NotFound("Announcement not found.");
            }

            // Map to DTO
            var announcementDto = new AnnouncementsDTO
            {
                AnnouncementId = announcement.AnnouncementId,
                ContestId = announcement.ContestId,
                UserId = announcement.UserId,
                Message = announcement.Message,
                CreatedAt = announcement.CreatedAt
            };

            return Ok(announcementDto);
        }

        // GET: /api/contests/{contestId}/standings
        [HttpGet("{contestId}/standings")]
        public async Task<ActionResult> GetStandings(int contestId)
        {
            var contest = await _context.Contests
                .FirstOrDefaultAsync(c => c.ContestId == contestId);

            if (contest == null)
            {
                return NotFound("Contest not found.");
            }

            var submissions = await _context.Submission
                .Include(s => s.User)
                .Include(s => s.Problem)
                .Where(s => s.Problem.ContestId == contestId)
                .ToListAsync();

            var standings = CalculateStandings(submissions, contest);

            if (!standings.Any())
            {
                return Ok(new { Message = "No standings available for this contest." });
            }

            return Ok(standings);
        }

        private List<StandingDto> CalculateStandings(List<Submission> submissions, Contest contest)
        {
            var standings = new List<StandingDto>();
            var userSubmissions = submissions.GroupBy(s => s.UserId);

            foreach (var userGroup in userSubmissions)
            {
                var userId = userGroup.Key;
                var user = userGroup.First().User;
                var solvedProblems = new HashSet<int>();
                int totalPenalty = 0;
                int solvedCount = 0;

                foreach (var submission in userGroup.OrderBy(s => s.SubmittedAt))
                {
                    var problemId = submission.ProblemId;

                    if (!solvedProblems.Contains(problemId))
                    {
                        if (submission.Result == "Accepted")
                        {
                            solvedProblems.Add(problemId);
                            solvedCount++;

                            // Calculate penalty: time since contest start + 20 minutes per wrong attempt
                            var wrongAttempts = userGroup
                                .Where(s => s.ProblemId == problemId && s.SubmittedAt < submission.SubmittedAt && s.Result != "Accepted")
                                .Count();

                            var timeSinceStart = (submission.SubmittedAt - contest.BeginTime).TotalMinutes;
                            totalPenalty += (int)timeSinceStart + (wrongAttempts * 20);
                        }
                    }
                }

                standings.Add(new StandingDto
                {
                    UserId = userId,
                    Username = user.Username,
                    SolvedCount = solvedCount,
                    Penalty = totalPenalty
                });
            }

            // Sort by solved count (descending) then penalty (ascending)
            return standings.OrderByDescending(s => s.SolvedCount)
                           .ThenBy(s => s.Penalty)
                           .ToList();
        }
    }

    public class StandingDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int SolvedCount { get; set; }
        public int Penalty { get; set; }
    }

    public class CreateAnnouncementResponse
    {
        public string Message { get; set; }
        public AnnouncementsDTO Announcement { get; set; }
    }
}
>>>>>>> 33bc491bac067e804c9a612a8d4291ed08924930
