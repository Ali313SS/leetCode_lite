using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AJudge.Application.DTO.AuthDTO;
using AJudge.Application.DTO.UserDTOS;
using System.Reflection.Metadata.Ecma335;

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




       







    }
}
