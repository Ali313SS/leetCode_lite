using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Problem
    {
        public int ProblemId { get; set; }
        public int ContestId { get; set; }
        public string ProblemName { get; set; }
        public string? ProblemSource { get; set; }
        public int ProblemSourceID{ get; set; }
        public string ProblemLink { get; set; }
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public int numberOfTestCases { get; set; }
        public int Rating { get; set; }
               
        // Navigation Properties
        [ForeignKey("ContestId")]
        Contest Contest { get; set; }
         public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        //public ICollection<string> stringTags { get; set; } = new List<string>();


        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
        public ICollection<ProblemTag> ProblemTags { get; set; } = new List<ProblemTag>();

    }
}
