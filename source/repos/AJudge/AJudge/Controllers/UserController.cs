using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AJudge.Application.DTO.AuthDTO;
using AJudge.Application.DTO.UserDTOS;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]   // get only the info of user no other related data like groups or friends etc
        public async Task<IActionResult> GetAllUsers()
        {
            List<User>? allUsers = await _context.Users.AsNoTracking().ToListAsync();
       
            if (allUsers.Count == 0)
                return NoContent();

            List<UserResponseDTO> userResponseDTO =allUsers.Select(x=>UserResponseDTO.ConvertToUserResponse(x)).ToList();
            
          return Ok(userResponseDTO);     
        }

        [HttpGet("{name}")]   
        public async Task<IActionResult> GetUser(string name)
        {
            User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == name);

            if (user == null)
                return NotFound("No Such User");

            UserResponseDTO userResponseDTO = UserResponseDTO.ConvertToUserResponse(user);

            return Ok(userResponseDTO);
        }



                  



        [HttpGet("GetUserGroupsInfo/{id:int}")]
        public async Task<IActionResult> GetUserGroupsName(int id)
        {
            User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                return NotFound("No Such User");


            //grt your groups
            var groupTask = await _context.Groups
              .Where(g => g.Members.Any(gm => gm.UserId == id))
              .Select(g => g.Name)
              .ToListAsync();
            UserResponse_GroupsNameDTO res= new UserResponse_GroupsNameDTO(groupTask);
            return Ok(res);

        }

        [HttpGet("GetUserCoachesInfo/{id:int}")]
        public async Task<IActionResult> GetUserCoachesName(int id)
        {
            User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                return NotFound("No Such User");


             var coachTask = await  _context.UserCoaches
               .Where(uc => uc.UserId == id)
               .Select(uc => uc.Coach.Username)
               .ToListAsync();

            var res = new UserResponse_CoachesNameDTO(coachTask);
            return Ok(res);

        }

         [HttpGet("GetUserTrainerInfo/{id:int}")]
        public async Task<IActionResult> GetUserTrainersName(int id)
        {
            User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                return NotFound("No Such User");


            
                var trainersTask = await _context.UserCoaches.Where(x => x.CoachId == id)
                     .Select(x => x.User.Username).ToListAsync();

            var res = new UserResponse_TrainersNameDTO(trainersTask);
            return Ok(res);

        }

         [HttpGet("GetUserFriendsInfo/{id:int}")]
        public async Task<IActionResult> GetUserFriendsName(int id)
        {
            User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                return NotFound("No Such User");



          
                var friendsTask =await   _context.UserFriend.Where(x => x.FriendId == id)
                    .Select(x => x.User.Username).ToListAsync();


            var res = new UserResponse_FriendsNameDTO(friendsTask);
            return Ok(res);

        }
        [HttpPost("Request-forCoach")]
        [Authorize]
        public async Task<IActionResult> RequestForCoach(string userName)
        {
            if (userName == null)
                return BadRequest("User data is null.");
            var userId = GetUserIdFromToken();
            var Coach = await _context.Users.FirstOrDefaultAsync(x => x.Username == userName);
            if (Coach == null)
                return NotFound("No Such User");
            if (Coach.UserId == userId)
            {
                return BadRequest("You cannot be your own coach");
            }
            var existingRequest = await _context.CoachRequests
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CoachId == Coach.UserId);
            if (existingRequest != null)
            {
                return BadRequest("You have already requested this coach.");
            }
            var result = await _context.CoachRequests.AddAsync(new CoachRequest
            {
                UserId = userId,
                CoachId = Coach.UserId
            });
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("Accept-Student")]
        [Authorize]
        public async Task<IActionResult> AcceptStudent(string userName)
        {
            if (userName == null)
                return BadRequest("User data is null.");
            var userId = GetUserIdFromToken();
            var student = await _context.Users.FirstOrDefaultAsync(x => x.Username == userName);
            if (student == null)
                return NotFound("No Such User");           
            var existingRequest = await _context.CoachRequests
                .FirstOrDefaultAsync(uc => uc.UserId == student.UserId && uc.CoachId == userId);
            if (existingRequest != null)
            {
               _context.CoachRequests.Remove(existingRequest);
                _context.UserCoaches.Add(new UserCoaches
                {
                    UserId = student.UserId,
                    CoachId = userId
                });
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("You have not requested this coach.");
        }
        [HttpPost("Reject-Student")]
        [Authorize]
        public async Task<IActionResult> RejectStudent(string userName)
        {
            if (userName == null)
                return BadRequest("User data is null.");
            var userId = GetUserIdFromToken();
            var student = await _context.Users.FirstOrDefaultAsync(x => x.Username == userName);
            if (student == null)
                return NotFound("No Such User");
            var existingRequest = await _context.CoachRequests
                .FirstOrDefaultAsync(uc => uc.UserId == student.UserId && uc.CoachId == userId);
            if (existingRequest != null)
            {
                _context.CoachRequests.Remove(existingRequest);
                await _context.SaveChangesAsync();
                return Ok("okey");
            }
            return BadRequest("You have not requested this coach.");
        }
        [HttpPost("Remove-Student")]
        [Authorize]
        public async Task<IActionResult> RemoveStudent(string userName)
        {
            if (userName == null)
                return BadRequest("User data is null.");
            var userId = GetUserIdFromToken();
            var student = await _context.Users.FirstOrDefaultAsync(x => x.Username == userName);
            if (student == null)
                return NotFound("No Such User");
            var existingRequest = await _context.UserCoaches
                .FirstOrDefaultAsync(uc => uc.UserId == student.UserId && uc.CoachId == userId);
            if (existingRequest != null)
            {
                _context.UserCoaches.Remove(existingRequest);
                await _context.SaveChangesAsync();
                return Ok(existingRequest);
            }
            return BadRequest("You have not requested this coach.");
        }
        [HttpPost("Remove-Coach")]
        [Authorize]
        public async Task<IActionResult> RemoveCoach(string userName)
        {
            if (userName == null)
                return BadRequest("User data is null.");
            var userId = GetUserIdFromToken();
            var coach = await _context.Users.FirstOrDefaultAsync(x => x.Username == userName);
            if (coach == null)
                return NotFound("No Such User");
            var existingRequest = await _context.UserCoaches
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CoachId == coach.UserId);
            if (existingRequest != null)
            {
                _context.UserCoaches.Remove(existingRequest);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("You have not requested this coach.");
        }
        [HttpGet("View-MyRequestsToCoach")]
        [Authorize]
        public async Task<IActionResult> ViewMyRequestsToCoach()
        {
            var userId = GetUserIdFromToken();
            var requests = await _context.CoachRequests
                .Where(uc => uc.CoachId == userId)
                .Select(uc => new { StudentUserName = uc.User.Username})
                .ToListAsync();
            if (requests.Count == 0)
                return NoContent();
            return Ok(requests);
        }
        [HttpGet("View-StudentRequests")]
        [Authorize]
        public async Task<IActionResult> ViewMyRequests()
        {
            var userId = GetUserIdFromToken();
            var requests = await _context.CoachRequests
                .Where(uc => uc.UserId == userId)
                .Select(uc => new {CoachUserName=uc.Coach.Username })
                .ToListAsync();
            if (requests.Count == 0)
                return NoContent();
            return Ok(requests);
        }
        [HttpGet("View-MyCoaches")]
        [Authorize]
        public async Task<IActionResult> ViewMyCoaches()
        {
            var userId = GetUserIdFromToken();
            var coaches = await _context.UserCoaches
                .Where(uc => uc.UserId == userId)
                .Select(uc => new {CoachUserName = uc.Coach.Username })
                .ToListAsync();
            if (coaches.Count == 0)
                return NoContent();
            return Ok(coaches);
        }

        [HttpGet("View-MyStudents")]
        [Authorize]
        public async Task<IActionResult> ViewMyStudents()
        {
            var userId = GetUserIdFromToken();
            var students = await _context.UserCoaches
                .Where(uc => uc.CoachId == userId)
                 .Select(uc => new { StudentUserName = uc.User.Username})
                .ToListAsync();
            if (students.Count == 0)
                return NoContent();
            return Ok(students);
        }
      
        private int GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                return int.Parse(userIdClaim.Value);
            }
            throw new UnauthorizedAccessException("User ID not found in token.");
        }














    }
}
