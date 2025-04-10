using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class UserCoach
    {
        public int UserCoachId { get; set; }
        public int UserId { get; set; }
        public int CoachId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Coach Coach { get; set; }
    }
}
