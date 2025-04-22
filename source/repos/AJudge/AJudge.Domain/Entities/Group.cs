using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string? ProfilePicture { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public int LeaderUserId { get; set; }
        // Navigation Properties

        [ForeignKey("LeaderUserId")]
        public User Leader { get; set; }
        public  ICollection<User> Members { get; set; } = new List<User>();
        public  ICollection<User> Managers { get; set; } = new List<User>();
        public ICollection<User> RequestsTojoinGroup { get; set; } = new List<User>();
        public ICollection<Contest> Contests { get; set; } = new List<Contest>();

    }
    public enum PrivacyType
    {
        Public,
        Private
    }
}
