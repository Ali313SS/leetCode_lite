using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AJudge.Domain.Entities
{
    public class Announcement
    {
        [Key]
        public int AnnouncementId { get; set; }
        public int UserId { get; set; }
        public int ContestId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("ContestId")]
        public Contest Contest { get; set; }
    }
}
