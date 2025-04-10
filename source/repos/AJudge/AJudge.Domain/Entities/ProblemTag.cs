using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class ProblemTag
    {
        public int ProblemTagId { get; set; }
        public int ProblemId { get; set; }
        public int TagId { get; set; }

        // Navigation Properties
        public Problem Problem { get; set; }
        public Tag Tag { get; set; }
    }
}
