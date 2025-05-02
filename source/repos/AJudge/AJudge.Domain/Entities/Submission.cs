using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Submission
    {
        public int SubmissionId { get; set; }
        public int UserId { get; set; }
        public int ProblemId { get; set; }     
        public DateTime SubmittedAt { get; set; }
        public string Result { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("ProblemId")]
        public Problem Problem { get; set; }    
        

        public int? GroupId { get; set; }

        [ForeignKey(nameof(Submission.GroupId))]
        public Group Group { get; set; }      
    }
}
