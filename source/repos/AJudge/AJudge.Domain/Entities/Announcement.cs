using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }
        public int UserId { get; set; }
        public int ContestId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Contest Contest { get; set; }
    }
}
