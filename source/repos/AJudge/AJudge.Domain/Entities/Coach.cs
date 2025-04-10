using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Coach
    {
        
            public int CoachId { get; set; }
            public int UserId { get; set; }
            public int Rank { get; set; }

            // Navigation Properties
            public User User { get; set; }
            public ICollection<UserCoach> UserCoaches { get; set; } = new List<UserCoach>();
        
    }
}
