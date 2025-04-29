using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO.AnnouncementsDTO
{
    public class AnnouncementsDTO
    {
        public int AnnouncementId { get; set; }
        public int ContestId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
