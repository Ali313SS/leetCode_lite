using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AJudge.Application.services;
using AJudge.Application.DTO.GroupDTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AJudge.Domain.Entities;
using Azure;
namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupServices _groupServices;
        public GroupController(IGroupServices groupServices)
        {
            _groupServices = groupServices;
        }

        [HttpPost]
        [Route("createGroup")]
        public async Task<ActionResult<GroupReturnDTO>> CreateGroup([FromBody] GroupDTO group)
        {
            if (group == null)
                return BadRequest("Group data is null.");
            var userId = GetUserIdFromToken();
            group.GroupLeader = userId;
            var result = await _groupServices.CreateGroup(group);
            return result;
            return BadRequest("Group creation failed.");
        }

   

        [HttpDelete]
        [Route("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var userId = GetUserIdFromToken();
            if (_groupServices.UserLeaderGroup(id, userId).Result == false)
            {
                return Unauthorized("You are not the leader of this group.");
            }
     
            var result = await _groupServices.DeleteGroup(id);
            if (result)
                return Ok("Group deleted successfully.");
            return NotFound("Group not found.");
        }

        [HttpPost]
        [Route("AddMember")]
        public async Task<IActionResult> AddMember(int groupId, int userId)
        {
            var userIdFromToken = GetUserIdFromToken();
            if (_groupServices.UserManagerInGroup(groupId, userIdFromToken).Result == false)
            {
                return Unauthorized("You are not a manager of this group.");
            }
            var result = await _groupServices.AddMember(groupId, userId);
            if (result)
                return Ok("Member added.");
            return BadRequest("Failed to add member.");
        }
        [HttpPost]
        [Route("RemoveMember")]
        public async Task<IActionResult> RemoveMember(int groupId, int userId)
        {
            var userIdFromToken = GetUserIdFromToken();
            if (_groupServices.UserManagerInGroup(groupId, userIdFromToken).Result == false)
            {
                return Unauthorized("You are not a manager of this group.");
            }
            var result = await _groupServices.RemoveMember(groupId, userId);
            if (result)
                return Ok("Member removed.");
            return BadRequest("Failed to remove member.");
        }
        
        [HttpPost]
        [Route("JoinGroup")]
        public async Task<IActionResult> JoinGroup(int groupId)
        {
            var userId = GetUserIdFromToken();
            string message = await _groupServices.joinGroup(groupId, userId);
            return Ok(message);
        }
        [HttpPost]
        [Route("LeaveGroup")]
        public async Task<IActionResult> LeaveGroup(int groupId, int userId)
        {
            var result = await _groupServices.LeaveGroup(groupId, userId);
            if (result)
                return Ok("Left group successfully.");
            return BadRequest("Failed to leave group.");
        }

        [HttpPost]
        [Route("AddManager")]
        public async Task<IActionResult> AddManager(int groupId, int userId)
        {
            var userIdFromToken = GetUserIdFromToken();
            if (_groupServices.UserManagerInGroup(groupId, userIdFromToken).Result == false)
            {
                return Unauthorized("You are not a manager of this group.");
            }
            var result = await _groupServices.AddManager(groupId, userId);
            if (result)
                return Ok("Manager added.");
            return BadRequest("Failed to add manager.");
        }

        [HttpPost]
        [Route("RemoveManager")]
        public async Task<IActionResult> RemoveManager(int groupId, int userId)
        {
            var userIdFromToken = GetUserIdFromToken();
            if (_groupServices.UserManagerInGroup(groupId, userIdFromToken).Result == false)
            {
                return Unauthorized("You are not a manager of this group.");
            }
            var result = await _groupServices.RemoveManager(groupId, userId);
            if (result)
                return Ok("Manager removed.");
            return BadRequest("Failed to remove manager.");
        }
        [HttpPut]
        [Route("DisableManager")]
        public async Task<IActionResult> DisableManager(int groupId, int userId)
        {
            var userIdFromToken = GetUserIdFromToken();
            if (_groupServices.UserManagerInGroup(groupId, userIdFromToken).Result == false)
            {
                return Unauthorized("You are not a manager of this group.");
            }
            var result = await _groupServices.DisableManager(groupId, userId);
            if (result)
                return Ok("Manager disabled.");
            return BadRequest("Failed to disable manager.");
        }
        [HttpPost]
        [Route("AcceptRequest")]

        public async Task<IActionResult> AcceptRequest(int groupId, int userId)
        {
            var userIdFromToken = GetUserIdFromToken();
            if (_groupServices.UserManagerInGroup(groupId, userIdFromToken).Result == false)
            {
                return Unauthorized("You are not a manager of this group.");
            }
            var result = await _groupServices.AcceptRequest(groupId, userId);
            if (result)
                return Ok("Request accepted.");
            return BadRequest("Failed to accept request.");
        }
        [HttpPost]
        [Route("RejectRequest")]

        public async Task<IActionResult> RejectRequest(int groupId, int userId)
        {
            var userIdFromToken = GetUserIdFromToken();
            if (_groupServices.UserManagerInGroup(groupId, userIdFromToken).Result == false)
            {
                return Unauthorized("You are not a manager of this group.");
            }
            var result = await _groupServices.RejectRequest(groupId, userId);
            if (result)
                return Ok("Request rejected.");
            return BadRequest("Failed to reject request.");
        }
        

        [HttpGet]
        [Route("GetGroupById")]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var userId = GetUserIdFromToken();
            if (_groupServices.UserMemberInGroup(id, userId).Result == false)
            {
                return Unauthorized("You are not a member of this group.");
            }
            var group = await _groupServices.GetGroupById(id);
            if (group != null)
                return Ok(group);
            return NotFound("Group not found.");
        }

        [HttpGet]
        [Route("members")]
        public async Task<IActionResult> GroupMembers(int groupId)
        {
            var userId = GetUserIdFromToken();
            if (_groupServices.UserMemberInGroup(groupId, userId).Result == false)
            {
                return Unauthorized("You are not a member of this group.");
            }
            
            var members = await _groupServices.GroupMembers(groupId);
            if (members != null)
                return Ok(members);
            return NotFound("Group not found.");
        }




        private int GetUserIdFromToken()
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            throw new UnauthorizedAccessException("User ID not found in token.");
        }
    }
}
