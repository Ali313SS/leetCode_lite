using AJudge.Application.DTO.GroupDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public interface IGroupServices
    {
        Task<GroupReturnDTO> GetGroupById(int id);
        Task<String> joinGroup(int groupId, int userId);
        Task<bool> LeaveGroup(int groupId, int userId);
        Task<bool> CreateGroup(GroupDTO group);
        Task<bool> UpdateGroup(GroupDTO group);
        Task<bool> DeleteGroup(int id);
        Task<bool> RemoveMember(int groupId, int userId);
        Task<bool> AddMember(int groupId, int userId);
        Task<bool> AddManager(int groupId, int userId);
        Task<bool> RemoveManager(int groupId, int userId);
        Task<bool> AcceptRequest(int groupId, int userId);
        Task<bool> RejectRequest(int groupId, int userId);
        Task<List<MemberDTO>> GroupMembers(int groupId);
        Task<bool>UserMemberInGroup(int groupId, int userId);
        Task<bool> UserManagerInGroup(int groupId, int userId);
        Task<bool> UserLeaderGroup(int groupId, int userId);

    }
}
