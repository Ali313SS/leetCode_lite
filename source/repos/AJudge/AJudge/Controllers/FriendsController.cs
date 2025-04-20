using System.Linq;
using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class FriendsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FriendsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest request)
        {
            var currentUser = await _context.Users.FindAsync(request.UserId);
            var friend = await _context.Users.FindAsync(request.FriendId);

            if (currentUser == null || friend == null || currentUser.UserId == request.FriendId)
            {
                return BadRequest("طلب غير صالح.");
            }

            // Check if the friendship already exists
            var existingFriendship = await _context.UserFriend
                .AnyAsync(uf => (uf.UserId == currentUser.UserId && uf.FriendId == request.FriendId) ||
                                (uf.UserId == request.FriendId && uf.FriendId == currentUser.UserId));

            if (existingFriendship)
            {
                return BadRequest("الصديق موجود بالفعل.");
            }

            // Add mutual friendship
            var userFriend = new UserFriend
            {
                UserId = currentUser.UserId,
                FriendId = request.FriendId
            };

            var mutualFriend = new UserFriend
            {
                UserId = request.FriendId,
                FriendId = currentUser.UserId
            };

            _context.UserFriend.Add(userFriend);
            _context.UserFriend.Add(mutualFriend);
            await _context.SaveChangesAsync();

            return Ok("تم إضافة الصديق بنجاح!");
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFriend([FromBody] RemoveFriendRequest request)
        {
            var currentUser = await _context.Users.FindAsync(request.UserId);
            var friend = await _context.Users.FindAsync(request.FriendId);

            if (currentUser == null || friend == null || currentUser.UserId == request.FriendId)
            {
                return BadRequest("طلب غير صالح.");
            }

            // Find the friendship entries (both directions)
            var userFriend = await _context.UserFriend
                .FirstOrDefaultAsync(uf => uf.UserId == currentUser.UserId && uf.FriendId == request.FriendId);

            var mutualFriend = await _context.UserFriend
                .FirstOrDefaultAsync(uf => uf.UserId == request.FriendId && uf.FriendId == currentUser.UserId);

            if (userFriend == null || mutualFriend == null)
            {
                return BadRequest("ليس صديقًا بالأساس.");
            }

            // Remove both friendship entries
            _context.UserFriend.Remove(userFriend);
            _context.UserFriend.Remove(mutualFriend);
            await _context.SaveChangesAsync();

            return Ok("تم إزالة الصديق بنجاح!");
        }
    }

    public class AddFriendRequest
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
    }

    public class RemoveFriendRequest
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
    }
}


//////////////////////////////////////  ده اللي نعمله ف الاخر ////////////////////////////////////

/*
 using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

            // Check if the friendship already exists
            var existingFriendship = await _context.UserFriends
                .AnyAsync(uf => (uf.UserId == currentUser.UserId && uf.FriendId == friendId) ||
                                (uf.UserId == friendId && uf.FriendId == currentUser.UserId));

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

            var mutualFriend = new UserFriend
            {
                UserId = friendId,
                FriendId = currentUser.UserId
            };

            _context.UserFriends.Add(userFriend);
            _context.UserFriends.Add(mutualFriend);
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

            // Find the friendship entries (both directions)
            var userFriend = await _context.UserFriends
                .FirstOrDefaultAsync(uf => uf.UserId == currentUser.UserId && uf.FriendId == friendId);

            var mutualFriend = await _context.UserFriends
                .FirstOrDefaultAsync(uf => uf.UserId == friendId && uf.FriendId == currentUser.UserId);

            if (userFriend == null || mutualFriend == null)
            {
                return BadRequest("This user is not in your friends list.");
            }

            // Remove both friendship entries
            _context.UserFriends.Remove(userFriend);
            _context.UserFriends.Remove(mutualFriend);
            await _context.SaveChangesAsync();

            return Ok("Friend removed successfully!");
        }
    }
 */