using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AJudge.Application.DtO.UserDTOS;

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

       

        [HttpGet("GetUserRelatedInfo/{id:int}")]  
        public async Task<IActionResult> GetUserGroups(int id) 
        {
            User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id);
            if (user == null)
                return NotFound("No Such User");


            //grt your groups
            var groupTask =  _context.Groups
              .Where(g => g.Members.Any(gm => gm.UserId == id))
              .Select(g => g.Name)
              .ToListAsync();


           


            //get your coaches
            var coachTask =  _context.UserCoaches
              .Where(uc => uc.UserId == id)
              .Select(uc => uc.Coach.Username)
              .ToListAsync();


            //get youe trainers
            var trainersTask =  _context.UserCoaches.Where(x => x.CoachId == id)
                 .Select(x => x.User.Username).ToListAsync();


            //get your friends
            var friendsTask =  _context.UserFriend.Where(x => x.FriendId == id)
                .Select(x => x.User.Username).ToListAsync();

            await Task.WhenAll(groupTask, coachTask, trainersTask, friendsTask);

            var goupNames = groupTask.Result;
            var coachNames = coachTask.Result;
            var trainerNames = trainersTask.Result;
            var friendNames = friendsTask.Result;


            UserResponse_RelatedInfoDTO userResponse = UserResponse_RelatedInfoDTO.ConvertToUser_RelatedInfoResponse(user, goupNames, coachNames, trainerNames, friendNames);
            return Ok(userResponse);
        }



      





    }
}
