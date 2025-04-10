using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
   public class Group
    {
        
            public int GroupId { get; set; }
            public string Name { get; set; }
            public PrivacyType Privacy { get; set; }
            public byte[] ProfilePicture { get; set; }
            public DateTime Created { get; set; }
            public int LeaderUserId { get; set; }

            // Navigation Properties
            public User Leader { get; set; }
            public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
            public ICollection<GroupChat> GroupChats { get; set; } = new List<GroupChat>();
            public ICollection<GroupContest> GroupContests { get; set; } = new List<GroupContest>();
        
    }
    public enum PrivacyType
    {
        Public,
        Private
    }
}
