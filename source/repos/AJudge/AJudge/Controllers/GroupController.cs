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

        /// <summary>
        /// Creates a new group with the given details.
        /// </summary>
        /// <param name="group">The group data including name, privacy, description, and optional profile picture.</param>
        /// <returns>
        /// Returns the created group as <see cref="GroupReturnDTO"/> on success.<br/>
        /// Returns 400 Bad Request if the group data is null or creation fails.
        /// </returns>
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


        /// <summary>
        /// Deletes a group by its ID if the current user is the group leader 
        /// </summary>
        /// <param name="id">The unique identifier of the group to delete.</param>
        /// <returns>
        /// Returns 200 OK with a success message if the group is deleted successfully.<br/>
        /// Returns 401 Unauthorized if the current user is not the leader of the group.<br/>
        /// Returns 404 Not Found if the group does not exist.
        /// </returns>
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


        /// <summary>
        /// Adds a user as a member to a specified group.
        /// </summary>
        /// <param name="groupId">The ID of the group to which the user will be added.</param>
        /// <param name="userId">The ID of the user to add as a member.</param>
        /// <returns>
        /// Returns 200 OK if the member was successfully added.<br/>
        /// Returns 401 Unauthorized if the current user is not a manager of the group.<br/>
        /// Returns 400 Bad Request if the member could not be added.
        /// </returns>
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

        /// <summary>
        /// Removes a user from the specified group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user to be removed from the group.</param>
        /// <returns>
        /// Returns 200 OK if the member was removed successfully.<br/>
        /// Returns 401 Unauthorized if the current user is not a manager of the group.<br/>
        /// Returns 400 Bad Request if the member removal failed.
        /// </returns>
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

        /// <summary>
        /// Allows the current authenticated user to join a specified group.
        /// </summary>
        /// <param name="groupId">The ID of the group to join.</param>
        /// <returns>
        /// Returns 200 OK with a message indicating the result of the join operation.
        /// </returns>
        [HttpPost]
        [Route("JoinGroup")]
        public async Task<IActionResult> JoinGroup(int groupId)
        {
            var userId = GetUserIdFromToken();
            string message = await _groupServices.joinGroup(groupId, userId);
            return Ok(message);
        }


        /// <summary>
        /// Allows a user to leave a specified group.
        /// </summary>
        /// <param name="groupId">The ID of the group to leave.</param>
        /// <param name="userId">The ID of the user leaving the group.</param>
        /// <returns>
        /// Returns 200 OK if the user left the group successfully.<br/>
        /// Returns 400 Bad Request if the operation failed.
        /// </returns>
        [HttpPost]
        [Route("LeaveGroup")]
        public async Task<IActionResult> LeaveGroup(int groupId, int userId)
        {
            var result = await _groupServices.LeaveGroup(groupId, userId);
            if (result)
                return Ok("Left group successfully.");
            return BadRequest("Failed to leave group.");
        }

        /// <summary>
        /// Adds a user as a manager to the specified group,the auth user should be the manager of the group
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user to be added as a manager.</param>
        /// <returns>
        /// Returns 200 OK if the user was added as a manager successfully.<br/>
        /// Returns 401 Unauthorized if the current user is not a manager of the group.<br/>
        /// Returns 400 Bad Request if the operation failed.
        /// </returns>
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


        /// <summary>
        /// Removes a manager from the specified group,the auth user should be manager
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the manager to remove.</param>
        /// <returns>
        /// Returns 200 OK if the manager was removed successfully.<br/>
        /// Returns 401 Unauthorized if the current user is not a manager of the group.<br/>
        /// Returns 400 Bad Request if the operation failed.
        /// </returns>
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

        /// <summary>
        /// Disables a manager role for a specific user in the given group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user whose manager role will be disabled.</param>
        /// <returns>
        /// Returns 200 OK if the manager was successfully disabled.<br/>
        /// Returns 401 Unauthorized if the current user is not a manager of the group.<br/>
        /// Returns 400 Bad Request if the operation failed.
        /// </returns>
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

        /// <summary>
        /// Accepts a user's join request to a specific group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user whose join request is being accepted.</param>
        /// <returns>
        /// Returns 200 OK if the request was accepted successfully.<br/>
        /// Returns 401 Unauthorized if the current user is not a manager of the group.<br/>
        /// Returns 400 Bad Request if the operation failed.
        /// </returns>
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

        /// <summary>
        /// Rejects a user's join request to a specific group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user whose join request is being rejected.</param>
        /// <returns>
        /// Returns 200 OK if the request was rejected successfully.<br/>
        /// Returns 401 Unauthorized if the current user is not a manager of the group.<br/>
        /// Returns 400 Bad Request if the operation failed.
        /// </returns>
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

        /// <summary>
        /// Retrieves detailed information about a group by its ID.
        /// </summary>
        /// <param name="id">The ID of the group to retrieve.</param>
        /// <returns>
        /// Returns 200 OK with the group data if found and the user is a member.<br/>
        /// Returns 401 Unauthorized if the current user is not a member of the group.<br/>
        /// Returns 404 Not Found if the group does not exist.
        /// </returns>
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

        /// <summary>
        /// Retrieves the list of members in a specified group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>
        /// Returns 200 OK with a list of members if the user is a member of the group.<br/>
        /// Returns 401 Unauthorized if the user is not a member of the group.<br/>
        /// Returns 404 Not Found if the group doesn't exist or has no members.
        /// </returns>
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
