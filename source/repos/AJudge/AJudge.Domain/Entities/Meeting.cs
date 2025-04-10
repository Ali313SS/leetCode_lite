using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Meeting
    {
           public int MeetingId { get; set; }
            public DateTime ScheduledTime { get; set; }
            public int CreatorUserId { get; set; }

            // Navigation Properties
            public User Creator { get; set; }
            public ICollection<UserMeeting> UserMeetings { get; set; } = new List<UserMeeting>();
        
    }
}
