using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    [NotMapped]
    public class RequestTojoinGroup
    {
        
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } // e.g., Pending, Accepted, Rejected
        // Navigation properties
    //    [ForeignKey("GroupId")]
        public  Group Group { get; set; }

      //  [ForeignKey("UserId")]
        public  User User { get; set; }
        // Constructor
    
    }
}
