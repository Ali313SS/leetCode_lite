using AJudge.Application.DTO.GroupDTO;
using AJudge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Domain.Entities;

namespace AJudge.Application.services
{
    public class GroupServices : IGroupServices
    {
        private readonly ApplicationDbContext _context;
        public GroupServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddMember(int groupId, int userId)
        {

            var uesr = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (uesr == null)
            {
                return await Task.FromResult(false);
            }
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null)
            {
                return await Task.FromResult(false);
            }
            if (group.Members.FirstOrDefault(u => u.UserId == userId) != null)
            {
                return await Task.FromResult(false);
            }  
            group.Members.Add(uesr);
            await _context.SaveChangesAsync();
            foreach (var i in group.Members)
            {
                Console.WriteLine($"Member Username: {i.Username}");
            }
            return await Task.FromResult(true);

        }
    
        public async Task<GroupReturnDTO> CreateGroup(GroupDTO group)
        {
            var newGroup = new Group
            {
                Name = group.Name,
                Privacy = group.Privacy,
                ProfilePicture = group.ProfilePicture,
                Description = group.Description,
                LeaderUserId = group.GroupLeader


            };
            await _context.Groups.AddAsync(newGroup);
            await _context.SaveChangesAsync();
            var groupreturn = new GroupReturnDTO
            {
                GroupId = newGroup.GroupId,
                Name = newGroup.Name,
                Privacy = newGroup.Privacy,
                ProfilePicture = newGroup.ProfilePicture,
                Description = newGroup.Description,
                Contests = newGroup.Contests.Select(c => new CContest
                {
                    Id = c.ContestId,
                    Name = c.Name,
                    Created = c.BeginTime,
                    Ended = c.EndTime
                }).ToList(),
            };
            return await Task.FromResult(groupreturn);

        }
        public async Task<bool> DeleteGroup(int id)
        {

            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == id);

            if (group == null)
            {
                return await Task.FromResult(false);
            }
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        public async Task<GroupReturnDTO> GetGroupById(int id)
        {

            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == id);
            if (group == null)
            {
                return null;
            }
            var groupDTO = new GroupReturnDTO
            {

                Name = group.Name,
                Privacy = group.Privacy,
                ProfilePicture = group.ProfilePicture,
                Description = group.Description,
                Contests = group.Contests.Select(c => new CContest
                {

                    Id = c.ContestId,
                    Name = c.Name,
                    Created = c.BeginTime,
                    Ended = c.EndTime

                }).ToList(),

            };
            return groupDTO;


        }
        public async Task<bool> LeaveGroup(int groupId, int userId)
        {
            var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null)
            {
                return await Task.FromResult(false);
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return await Task.FromResult(false);
            }
            if (!UserMemberInGroup(groupId,userId).Result)
            {
                return await Task.FromResult(false);
            }
            group.Members.Remove(user);
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        public async Task<bool> AddManager(int groupId, int userId)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return await Task.FromResult(false);
            }
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null)
            {
                return await Task.FromResult(false);
            }
            if (UserManagerInGroup(groupId,userId).Result)
            {
                return await Task.FromResult(false);
            }                    
            group.Members.Remove(user);
            group.Managers.Add(user);            
            await _context.SaveChangesAsync();
            return await Task.FromResult(true);

        }
        public async Task<bool> RemoveManager(int groupId, int userId)
        {
            
            _context.Groups.Include(g => g.Managers).ToList();
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null)
            {
                return await Task.FromResult(false);
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return await Task.FromResult(false);
            }
            if (group.Managers.FirstOrDefault(u => u.UserId == userId) == null)
            {
                return await Task.FromResult(false);
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    group.Managers.Remove(user);                    
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return await Task.FromResult(false);
                }
            }
            return await Task.FromResult(true);
        }
        public async Task<bool> DisableManager(int groupId, int userId)
        {
            var group = await _context.Groups
                .Include(g => g.Managers)
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (group == null)
            {
                return false;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null || !group.Managers.Any(u => u.UserId == userId))
            {
                return false;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                group.Managers.Remove(user);
                group.Members.Add(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }

            return true;
        }
        public async Task<bool> RemoveMember(int groupId, int userId)
        {
            return await LeaveGroup(groupId, userId);
        }
        public Task<bool> UpdateGroup(GroupDTO group)
        {
            throw new NotImplementedException();
        }

        public async Task<String> joinGroup(int groupId, int userId)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null)
            {
                return "Group not found";
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return "User not found";
            }
            
            if (UserMemberInGroup(groupId,userId).Result)
            {
                return "User already in group";
            }
            if (group.Privacy == PrivacyType.Private)
            {
                if (group.RequestsTojoinGroup.FirstOrDefault(u => u.UserId == userId) != null)
                {
                    return "Request already sent";
                }
                group.RequestsTojoinGroup.Add(user);
                await _context.SaveChangesAsync();
                return "Request sent to join group";
            }
            group.Members.Add(user);
            await _context.SaveChangesAsync();
            return "User added to group";
        }


        public async Task<bool> AcceptRequest(int groupId, int userId)
        {
            var group = await _context.Groups
                .Include(g => g.RequestsTojoinGroup)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);            
            if (group == null)
            {
                return await Task.FromResult(false);
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return await Task.FromResult(false);
            }
            if (group.RequestsTojoinGroup.FirstOrDefault(u => u.UserId == userId) == null)
            {
                return await Task.FromResult(false);
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    group.RequestsTojoinGroup.Remove(user);
                    group.Members.Add(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return await Task.FromResult(false);
                }
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> RejectRequest(int groupId, int userId)
        {
            var group = await _context.Groups
                .Include(g => g.RequestsTojoinGroup)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);        
            if (group == null)
            {
                return await Task.FromResult(false);
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return await Task.FromResult(false);
            }
            if (group.RequestsTojoinGroup.FirstOrDefault(u => u.UserId == userId) == null)
            {
                return await Task.FromResult(false);
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    group.RequestsTojoinGroup.Remove(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return await Task.FromResult(false);
                }
            }
            return await Task.FromResult(true);
        }

        public async Task<List<MemberDTO>> GroupMembers(int groupId)
        {
            var group = await _context.Groups
                .Include(g => g.Leader)
                .Include(g => g.Managers)
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (group == null)
            {
                return null;
            }

            var members = new List<MemberDTO>();

            if (group.Leader != null)
            {
                members.Add(new MemberDTO
                {
                    UserName = group.Leader.Username,
                    Role = "Leader",
                });
            }

            members.AddRange(group.Managers
                .Select(u => new MemberDTO
                {
                    UserName = u.Username,
                    Role = "Manager",
                })
                .OrderBy(m => m.UserName)
                .ToList());

            members.AddRange(group.Members
                .Select(u => new MemberDTO
                {
                    UserName = u.Username,
                    Role = "Member",
                })
                .OrderBy(m => m.UserName)
                .ToList());

            return members;
        }
        public async Task<bool> UserMemberInGroup(int groupId, int userId)
        {
         
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null)
            {
                return await Task.FromResult(false);
            }
            _context.Groups.Include(g => g.Members).ToList();
            _context.Groups.Include(g => g.Managers).ToList();
            var user = group.Members.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
                return true;
            return await UserManagerInGroup(groupId, userId);
        }
        public async Task<bool> UserManagerInGroup(int groupId, int userId)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null)
            {
                return await Task.FromResult(false);
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return await Task.FromResult(false);
            }
            _context.Groups.Include(g => g.Members).ToList();
            _context.Groups.Include(g => g.Managers).ToList();
            if (group.Managers.FirstOrDefault(u => u.UserId == userId) != null)
            {
                return await Task.FromResult(true);
            }

            return await UserLeaderGroup(groupId, userId);

        }

        public async Task<bool> UserLeaderGroup(int groupId, int userId)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null)
            {
                return await Task.FromResult(false);
            }
            if (group.LeaderUserId == userId)
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
    }


}
