using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class UserMeeting
    {
        public int UserMeetingId { get; set; }
        public int UserId { get; set; }
        public int MeetingId { get; set; }
        public DateTime StartedAt { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Meeting Meeting { get; set; }
    }
}
