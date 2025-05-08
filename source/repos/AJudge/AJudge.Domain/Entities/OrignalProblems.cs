using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class OrignalProblems
    {
        public string ProblemId { get; set; }
        public string ProblemSource { get; set; }
        public string ProblemName { get; set; }
        public int ProblemSourceID { get; set; }

        public string ProblemLink { get; set; }
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public int numberOfTestCases { get; set; }
        public int Rating { get; set; }


        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

        public virtual ICollection<OrignalTestCases> TestCases { get; set; } = new List<OrignalTestCases>();
        public virtual ICollection<ProblemTag> ProblemTags { get; set; } = new List<ProblemTag>();

    }
  

}
