using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class ContestProblem
    {
        public int ContestProblemId { get; set; }
        public int ContestId { get; set; }
        public int ProblemId { get; set; }

        // Navigation Properties
        public Contest Contest { get; set; }
        public Problem Problem { get; set; }
    }
}
