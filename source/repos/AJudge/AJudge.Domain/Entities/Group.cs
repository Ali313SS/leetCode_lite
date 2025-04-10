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
        public String ProfilePicture { get; set; }
        public DateTime Created { get; set; }
        public int LeaderUserId { get; set; }

        // Navigation Properties
        public User Leader { get; set; }
        public ICollection<User> UserGroups { get; set; } = new List<User>();
        public ICollection<User> GroupManagers { get; set; } = new List<User>();
        public ICollection<Contest> Contests { get; set; } = new List<Contest>();

    }
    public enum PrivacyType
    {
        Public,
        Private
    }
}
