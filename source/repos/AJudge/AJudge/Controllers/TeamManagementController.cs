using AJudge.Application.DtO;
using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(ApplicationDbContext context, ILogger<TeamsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TeamResponseDto>> CreateTeam(CreateTeamDto dto)
        {
            var currentUserId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(currentUserId);

            if (user == null)
            {
                return Unauthorized();
            }

            var team = new Team
            {
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow
            };

            _context.Teams.Add(team);

            var userTeam = new UserTeam
            {
                UserId = currentUserId,
                Team = team
            };

            _context.UserTeams.Add(userTeam);

            await _context.SaveChangesAsync();

            var response = new TeamResponseDto
            {
                TeamId = team.TeamId,
                Name = team.Name,
                CreatedAt = team.CreatedAt,
                Members = new List<TeamMemberDto>
            {
                new TeamMemberDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email
                }
            }
            };

            return CreatedAtAction(nameof(GetTeam), new { id = team.TeamId }, response);
        }

        [HttpPost("{teamId}/join")]
        public async Task<IActionResult> JoinTeam(int teamId)
        {
            var currentUserId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(currentUserId);
            var team = await _context.Teams.FindAsync(teamId);

            if (user == null)
            {
                return Unauthorized();
            }

            if (team == null)
            {
                return NotFound("Team not found");
            }

            var existingMembership = await _context.UserTeams
                .FirstOrDefaultAsync(ut => ut.UserId == currentUserId && ut.TeamId == teamId);

            if (existingMembership != null)
            {
                return BadRequest("User is already a member of this team");
            }

            var userTeam = new UserTeam
            {
                UserId = currentUserId,
                TeamId = teamId
            };

            _context.UserTeams.Add(userTeam);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{teamId}/leave")]
        public async Task<IActionResult> LeaveTeam(int teamId)
        {
            var currentUserId = GetCurrentUserId();

            var membership = await _context.UserTeams
                .FirstOrDefaultAsync(ut => ut.UserId == currentUserId && ut.TeamId == teamId);

            if (membership == null)
            {
                return NotFound("You are not a member of this team");
            }

            _context.UserTeams.Remove(membership);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamResponseDto>> GetTeam(int id)
        {
            var team = await _context.Teams
                .Include(t => t.UserTeams)
                .ThenInclude(ut => ut.User)
                .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null)
            {
                return NotFound();
            }

            var response = new TeamResponseDto
            {
                TeamId = team.TeamId,
                Name = team.Name,
                CreatedAt = team.CreatedAt,
                Members = team.UserTeams.Select(ut => new TeamMemberDto
                {
                    UserId = ut.User.UserId,
                    Username = ut.User.Username,
                    Email = ut.User.Email
                }).ToList()
            };

            return Ok(response);
        }


        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated or user ID is invalid");
            }
            return userId;
        }
    }
}