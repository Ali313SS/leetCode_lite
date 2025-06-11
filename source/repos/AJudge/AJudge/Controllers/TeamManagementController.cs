using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using AJudge.Application.DTO.TeamDTOS;

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
        /// <summary>
        /// Creates a new team and adds the authenticated user as a member of the team.
        /// </summary>
        /// <param name="dto">The data transfer object containing team Name</param>
        /// <returns>
        /// Returns an <see cref="ActionResult{TeamResponseDto}"/> containing the created team details including members.
        /// Returns Unauthorized if the current user is not found.
        /// Returns CreatedAtAction response with the newly created team info.
        /// </returns>
        [HttpPost("CreateTeam")]
        [Authorize]
        public async Task<ActionResult<TeamResponseDto>> CreateTeam([FromBody]CreateTeamDto dto)
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
            await _context.SaveChangesAsync();      
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
                }
            }
            };

            return CreatedAtAction(nameof(GetTeam), new { id = team.TeamId }, response);
        }
        [HttpDelete("")]
        [Authorize]
        public async Task<IActionResult>DeleteTeam(int teamId)
        {
            var currentUserId = GetCurrentUserId();            
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
            {
                return NotFound("Team not found");
            }
            var isUserInTeam = await _context.UserTeams
                .AnyAsync(ut => ut.UserId == currentUserId && ut.TeamId == teamId);
            if (!isUserInTeam)
            {
                return Unauthorized("You are not a member of this team");
            }
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return Ok("Team deleted successfully");
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Rename(int teamId, string newName)
        {
            var currentUserId = GetCurrentUserId();
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
            {
                return NotFound("Team not found");
            }
            var isUserInTeam = await _context.UserTeams
                .AnyAsync(ut => ut.UserId == currentUserId && ut.TeamId == teamId);
            if (!isUserInTeam)
            {
                return Unauthorized("You are not a member of this team");
            }
            team.Name =newName;
            await _context.SaveChangesAsync();
            return Ok("Team updated successfully");
        }
        [HttpGet("Inactive_member")]
        [Authorize]
        public async Task<IActionResult> GetInactiveMembers(int teamId)
        {
            var currentUserId = GetCurrentUserId();
            var isUserInTeam = await _context.UserTeams
                .AnyAsync(ut => ut.UserId == currentUserId && ut.TeamId == teamId);
            if (!isUserInTeam)
            {
                return Unauthorized("You are not a member of this team");
            }
            var inactiveMembers = await _context.UserTeamInvitations
                .Where(ut => ut.TeamId == teamId)
                .Select(ut => new TeamMemberDto
                {
                    UserId = ut.User.UserId,
                    Username = ut.User.Username,
                })
                .ToListAsync();
            return Ok(inactiveMembers);
        }

        /// <summary>
        /// Sends an invitation to a user to join a specified team.
        /// </summary>
        /// <param name="teamId">The ID of the team to which the user is invited.</param>
        /// <param name="username">The username of the user to be invited.</param>
        /// <returns>
        /// Returns <see cref="UnauthorizedResult"/> if the current user is not a member of the team.
        /// Returns <see cref="NotFoundResult"/> if the team or user does not exist.
        /// Returns <see cref="BadRequestObjectResult"/> if the user is already a member or has a pending invitation.
        /// Returns <see cref="OkObjectResult"/> upon successful invitation.
        /// </returns>
        [HttpPost("InviteUserToTeam")]
        [Authorize]
        public async Task<IActionResult> InviteUserToTeam(int teamId, string username)
        {
            var currentUserId = GetCurrentUserId();
            var isUserInTeam = await _context.UserTeams
                .AnyAsync(ut => ut.UserId == currentUserId && ut.TeamId == teamId);
            if (!isUserInTeam)
            {
                return Unauthorized("You are not a member of this team");
            }
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
            {
                return NotFound("Team not found");
            }
            var userToInvite = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (userToInvite == null)
            {
                return NotFound("User not found");
            }
            var isUserAlreadyInTeam = await _context.UserTeams
                .AnyAsync(ut => ut.UserId == userToInvite.UserId && ut.TeamId == teamId);
            if (isUserAlreadyInTeam)
            {
                return BadRequest("User is already a member of this team");
            }
            var existingInvitation = await _context.UserTeamInvitations
            .FirstOrDefaultAsync(ut => ut.UserId == userToInvite.UserId && ut.TeamId == teamId);
            if (existingInvitation != null)
            {
                return BadRequest("User already invited to this team");
            }
            var invitation = new UserTeamInvitation
            {
                UserId = userToInvite.UserId,
                TeamId = team.TeamId,
            };
            _context.UserTeamInvitations.Add(invitation);
            await _context.SaveChangesAsync();
            return Ok("Invitation sent successfully");
        }


        /// <summary>
        /// Allows the authenticated user to accept an invitation to join a specified team.
        /// </summary>
        /// <param name="teamId">The ID of the team for which the invitation is being accepted.</param>
        /// <returns>
        /// Returns <see cref="NotFoundResult"/> if no invitation exists for the user and team.
        /// Returns <see cref="OkObjectResult"/> upon successful acceptance of the invitation.
        /// </returns>
        [HttpPost("accept-invitation")]
        [Authorize]
        public async Task<IActionResult> AcceptInvitation(int teamId)
        {
            var currentUserId = GetCurrentUserId();
            var invitation = await _context.UserTeamInvitations
                .FirstOrDefaultAsync(ut => ut.UserId == currentUserId && ut.TeamId == teamId);
            if (invitation == null)
            {
                return NotFound("No invitation found for this team");
            }
            var membership = new UserTeam
            {
                UserId = currentUserId,
                TeamId = teamId
            };
            var team = await _context.Teams.FindAsync(teamId);
            _context.UserTeams.Add(membership);            
            _context.UserTeamInvitations.Remove(invitation);
            await _context.SaveChangesAsync();
            return Ok("Invitation accepted successfully");
        }

        /// <summary>
        /// Allows the authenticated user to reject an invitation to join a specified team.
        /// </summary>
        /// <param name="teamId">The ID of the team for which the invitation is being rejected.</param>
        /// <returns>
        /// Returns <see cref="NotFoundResult"/> if no invitation exists for the user and team.
        /// Returns <see cref="OkObjectResult"/> upon successful rejection and removal of the invitation.
        /// </returns>
        [HttpPost("reject-invitation")]
        [Authorize]
        public async Task<IActionResult> RejectInvitation(int teamId)
        {
            var currentUserId = GetCurrentUserId();
            var invitation = await _context.UserTeamInvitations
                .FirstOrDefaultAsync(ut => ut.UserId == currentUserId && ut.TeamId == teamId);
            if (invitation == null)
            {
                return NotFound("No invitation found for this team");
            }
            _context.UserTeamInvitations.Remove(invitation);
            await _context.SaveChangesAsync();
            return Ok("Invitation rejected successfully");
        }

        /// <summary>
        /// Allows the authenticated user to leave a specified team.
        /// </summary>
        /// <param name="teamId">The ID of the team to leave.</param>
        /// <returns>
        /// Returns <see cref="NotFoundResult"/> if the user is not a member of the specified team.
        /// Returns <see cref="OkResult"/> upon successful removal of the user from the team.
        /// </returns>
        [HttpPost("{teamId}/leave")]
        [Authorize]
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

        /// <summary>
        /// Retrieves detailed information about a specific team.
        /// </summary>
        /// <param name="id">The ID of the team to retrieve.</param>
        /// <returns>
        /// Returns <see cref="UnauthorizedResult"/> if the authenticated user is not a member of the specified team.
        /// Returns <see cref="NotFoundResult"/> if the team does not exist.
        /// Returns <see cref="OkObjectResult"/> with <see cref="TeamResponseDto"/> containing team details (TeamId,Name,Date of Creation , Members) 
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamResponseDto>> GetTeam(int id)
        {
            var currentUserId = GetCurrentUserId();
            var isUserInTeam = await _context.UserTeams
                .AnyAsync(ut => ut.UserId == currentUserId && ut.TeamId == id);
            if (!isUserInTeam)
            {
                return Unauthorized("You are not a member of this team");
            }

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
                }).ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a list of teams that the authenticated user is a member of.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="OkObjectResult"/> containing a list of <see cref="TeamResponseDto"/>,
        /// each representing a team containing(TeamId,Name,Date of Creation , Members)
        /// </returns>
        [HttpGet("MyTeams")]
        public async Task<ActionResult<IEnumerable<TeamResponseDto>>> MyTeams()
        {
            var currentUserId = GetCurrentUserId();
            var teams = await _context.UserTeams
                .Include(ut => ut.Team)
                .ThenInclude(t => t.UserTeams)
                .ThenInclude(ut => ut.User)
                .Where(ut => ut.UserId == currentUserId)
                .Select(ut => new TeamResponseDto
                {
                    TeamId = ut.Team.TeamId,
                    Name = ut.Team.Name,
                    CreatedAt = ut.Team.CreatedAt,
                    Members = ut.Team.UserTeams.Select(u => new TeamMemberDto
                    {
                        UserId = u.User.UserId,
                        Username = u.User.Username,                       
                    }).ToList()
                })
                .ToListAsync();
            return Ok(teams);
        }

        /// <summary>
        /// Retrieves a list of team invitations for the authenticated user.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="OkObjectResult"/> containing a list of <see cref="TeamResponseDto"/>,
        /// each representing a team the user has been invited to, including its members.
        /// </returns>
        [HttpGet("GetMyInvitations")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TeamResponseDto>>> GetMyInvitation()
        {
            var currentUserId = GetCurrentUserId();
            var invitations = await _context.UserTeamInvitations
                .Include(ut => ut.Team)
                .ThenInclude(t => t.UserTeams)
                .ThenInclude(ut => ut.User)
                .Where(ut => ut.UserId == currentUserId)
                .Select(ut => new TeamResponseDto
                {
                    TeamId = ut.Team.TeamId,
                    Name = ut.Team.Name,
                    CreatedAt = ut.Team.CreatedAt,
                    Members = ut.Team.UserTeams.Select(u => new TeamMemberDto
                    {
                        UserId = u.User.UserId,
                        Username = u.User.Username,
                    }).ToList()
                })
                .ToListAsync();
            return Ok(invitations);
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