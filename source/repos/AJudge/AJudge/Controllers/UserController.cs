using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AJudge.Application.DTO.AuthDTO;
using AJudge.Application.DTO.UserDTOS;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AJudge.Domain.RepoContracts;
using AJudge.Application.services;

namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public UserController(ApplicationDbContext context, IUnitOfWork unitOfWork, IUserService userService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _userService= userService;
        }

        /// <summary>
        /// Retrieves a list of all users with basic information only.
        /// Does not include related data such as groups, friends, or other navigation properties.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="OkObjectResult"/> containing a list of <see cref="UserResponseDTO"/> objects
        /// if users exist; otherwise, returns <see cref="NoContentResult"/>.
        /// </returns>
        [HttpGet]   // get only the info of user no other related data like groups or friends etc
        public async Task<IActionResult> GetAllUsers()
        {
            List<User>? allUsers = await _context.Users.AsNoTracking().ToListAsync();
       
            if (allUsers.Count == 0)
                return NoContent();

            List<UserResponseDTO> userResponseDTO =allUsers.Select(x=>UserResponseDTO.ConvertToUserResponse(x)).ToList();
            
          return Ok(userResponseDTO);     
        }

        /// <summary>
        /// Retrieves the public profile information of a user by their username.
        /// </summary>
        /// <param name="name">The username of the user to retrieve.</param>
        /// <returns>
        /// Returns 200 OK with user data if found; otherwise, 404 Not Found.
        /// </returns>
        [HttpGet("{name}")]   
        public async Task<IActionResult> GetUser(string name)
        {
            User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == name);

            if (user == null)
                return NotFound("No Such User");

            UserResponseDTO userResponseDTO = UserResponseDTO.ConvertToUserResponse(user);

            return Ok(userResponseDTO);
        }






        /// <summary>
        /// Retrieves the names of all groups that a specific user belongs to.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>
        /// Returns 200 OK with a list of group names if the user exists; otherwise, 404 Not Found.
        /// </returns>
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


        /// <summary>
        /// Retrieves the usernames of all coaches assigned to a specific user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>
        /// Returns 200 OK with a list of coach usernames if the user exists; otherwise, 404 Not Found.
        /// </returns>
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


        /// <summary>
        /// Retrieves the usernames of all users for whom the specified user is a coach
        /// </summary>
        /// <param name="id">The coach user ID.</param>
        /// <returns>
        /// Returns 200 OK with a list of usernames if the coach user exists; otherwise, 404 Not Found.
        /// </returns>
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

        /// <summary>
        /// Retrieves the list of friends (usernames) for a specific user.
        /// </summary>
        /// <param name="id">The ID of the user whose friends are to be retrieved.</param>
        /// <returns>
        /// A list of usernames who have added the specified user as a friend.
        /// Returns 404 if the user does not exist.
        /// </returns>
        /// <response code="200">Returns the list of friends' usernames.</response>
        /// <response code="404">If the specified user does not exist.</response>
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

        /// <summary>
        /// Sends a coaching request to the specified user.
        /// </summary>
        /// <param name="userName">The username of the user to request as a coach.</param>
        /// <returns>
        /// Returns 200 OK if the request is sent successfully, 
        /// 400 Bad Request if the input is invalid or a request already exists, 
        /// or 404 Not Found if the specified user does not exist.
        /// </returns>
        /// <remarks>
        /// The requesting user must be authenticated. A user cannot request themselves as a coach.
        /// </remarks>
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

        /// <summary>
        /// Accepts a coaching request from a student.
        /// </summary>
        /// <param name="userName">The username of the student who sent the request.</param>
        /// <returns>
        /// Returns 200 OK if the request is accepted and the student is added, 
        /// 400 Bad Request if no request exists or input is invalid, 
        /// or 404 Not Found if the student does not exist.
        /// </returns>
        /// <remarks>
        /// This endpoint is for coaches to accept requests sent by students. 
        /// The authenticated user must be the coach receiving the request.
        /// </remarks>
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

        /// <summary>
        /// Rejects a coaching request from a student.
        /// </summary>
        /// <param name="userName">The username of the student whose request is being rejected.</param>
        /// <returns>
        /// Returns 200 OK if the request is successfully rejected, 
        /// 400 Bad Request if no such request exists or input is invalid, 
        /// or 404 Not Found if the user does not exist.
        /// </returns>
        /// <remarks>
        /// This endpoint allows a coach (authenticated user) to reject a coaching request sent by a student.
        /// </remarks>
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

        /// <summary>
        /// Removes an existing student from the coach's list.
        /// </summary>
        /// <param name="userName">The username of the student to remove.</param>
        /// <returns>
        /// Returns 200 OK with the removed relationship if successful,  
        /// 400 Bad Request if no such student is assigned to the coach,  
        /// or 404 Not Found if the user does not exist.
        /// </returns>
        /// <remarks>
        /// This endpoint allows a coach (authenticated user) to remove an assigned student from their coaching list.
        /// </remarks>
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

        /// <summary>
        /// Removes the current user's coach by username.
        /// </summary>
        /// <param name="userName">The username of the coach to remove.</param>
        /// <returns>
        /// Returns 200 OK if the coach relationship is successfully removed,  
        /// 400 Bad Request if the coach was not assigned,  
        /// or 404 Not Found if the coach does not exist.
        /// </returns>
        /// <remarks>
        /// This endpoint allows a user (authenticated) to remove their assigned coach.
        /// </remarks>
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


        /// <summary>
        /// Retrieves all pending coaching requests sent to the currently authenticated user as a coach.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with a list of userId and CoachId who requested coaching,  
        /// or 204 No Content if there are no pending requests.
        /// </returns>
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


        /// <summary>
        /// Retrieves all pending coach requests made by the currently authenticated user.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with a list of coach userId & CoachId requested by the user,
        /// or 204 No Content if there are no pending requests.
        /// </returns>

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

        /// <summary>
        /// Retrieves the list of coaches associated with the currently authenticated user.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with a list of  UserCoaches have userId and coachId,
        /// or 204 No Content if the user has no coaches.
        /// </returns>
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

        /// <summary>
        /// Retrieves the list of students associated with the currently authenticated coach.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with a list of UserCoaches have userId and coachId,
        /// or 204 No Content if the coach has no students.
        /// </returns>
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


        /// <summary>
        /// Retrieves paginated list of users belonging to a specific club/university.
        /// </summary>
        /// <param name="clubName">The name of the club/university to filter users by.</param>
        /// <param name="isAssending">Whether to sort ascending by registration date (default false).</param>
        /// <param name="pageNNumber">Page number for pagination (default 1).</param>
        /// <param name="pageSSize">Page size for pagination (default 20).</param>
        /// <returns>A paginated list of users in the club with user Data (userName,Email,BirthDate,ProfilePic,LastSeen,ProblemTriedCount,Club,RegisterDate),and  TotalPage and PageNumber and HasNext and HasPrevious  </returns>
        [HttpGet("clubName/{clubName}")]
       public async Task<IActionResult> GetAllUseInClub(string clubName ,bool isAssending=false,int pageNNumber=1,int pageSSize=20)
       {
            User? clubExist = await _unitOfWork.User.GetSpecific(x => x.ClubUniversity == clubName,null);
            if (clubExist == null)
                return NoContent();

          
            UserPagination users = await _userService.GetAllUserInClubPerPage(nameof(AJudge.Domain.Entities.User.ClubUniversity),
                 clubName,nameof(AJudge.Domain.Entities.User.RegisterAt), isAssending, pageNNumber, pageSSize);

            var response = new
            {
                ItemsResponse = users.Items.Select(x => UserResponseDTO.ConvertToUserResponse(x)).ToList(),
                pagenumber = users.PageNumber,
                totalPages = users.TotalPages,
                hasPrevious = users.HasPrevious,
                hasNext = users.HasNext
            };

            return Ok(response);
       }

    }
}


