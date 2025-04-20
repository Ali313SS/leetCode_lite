
 using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FriendsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FriendsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(int friendId)
        {
            // Get current user ID from Claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("User not authenticated.");
            }

            var currentUser = await _context.Users.FindAsync(currentUserId);
            var friend = await _context.Users.FindAsync(friendId);

            if (currentUser == null || friend == null || currentUser.UserId == friendId)
            {
                return BadRequest("Invalid Request.");
            }

            var existingFriendship = await _context.UserFriend
                .AnyAsync(uf => (uf.UserId == currentUser.UserId && uf.FriendId == friendId));


            if (existingFriendship)
            {
                return BadRequest("Friend already exists.");
            }

            // Add mutual friendship
            var userFriend = new UserFriend
            {
                UserId = currentUser.UserId,
                FriendId = friendId
            };


            _context.UserFriend.Add(userFriend);
            await _context.SaveChangesAsync();
            return Ok("Friend added successfully!");
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFriend(int friendId)
        {
            // Get current user ID from Claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("User not authenticated.");
            }

            var currentUser = await _context.Users.FindAsync(currentUserId);
            var friend = await _context.Users.FindAsync(friendId);

            if (currentUser == null || friend == null || currentUser.UserId == friendId)
            {
                return BadRequest("Invalid Request.");
            }

            var userFriend = await _context.UserFriend
                .FirstOrDefaultAsync(uf => uf.UserId == currentUser.UserId && uf.FriendId == friendId);

            if (userFriend == null)
            {
                return BadRequest("This user is not in your friends list.");
            }

            // Remove both friendship entries
            _context.UserFriend.Remove(userFriend);
            await _context.SaveChangesAsync();

            return Ok("Friend removed successfully!");
        }
    }
}