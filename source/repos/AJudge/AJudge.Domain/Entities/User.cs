using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class User
    {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
            public DateTime? BirthDate { get; set; }
            public string ProfilePicture { get; set; }
            public DateTime? LastSeen { get; set; }
            public int ProblemsTriedCount { get; set; }
            public string ClubUniversity { get; set; }

            // Navigation Properties
            public ICollection<Team> Teams { get; set; } = new List<Team>();
            public ICollection<Group> Groups { get; set; } = new List<Group>();       
            public ICollection<Contest> CompeteContests { get; set; } = new List<Contest>();
            public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
            public ICollection<Vote> Votes { get; set; } = new List<Vote>();
            public ICollection<Statistics> Statistics { get; set; } = new List<Statistics>();
            public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
            public ICollection<Coach> UserCoaches { get; set; } = new List<Coach>();
        
    }
}
