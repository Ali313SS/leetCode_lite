using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class UserCoaches
    {
        public int UserId { get; set; }
        public int CoachId { get; set; }
        public User User { get; set; }
        public User Coach { get; set; }
    }
}
