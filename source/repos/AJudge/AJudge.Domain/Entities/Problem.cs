using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Problem
    {
        public int ProblemId { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public string Topic { get; set; }

        // Navigation Properties
        public ICollection<ContestProblem> ContestProblems { get; set; } = new List<ContestProblem>();
        public ICollection<ProblemTag> ProblemTags { get; set; } = new List<ProblemTag>();
    }
}
