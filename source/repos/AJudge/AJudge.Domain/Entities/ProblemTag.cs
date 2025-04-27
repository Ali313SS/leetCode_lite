using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class ProblemTag
    {
        public int ProblemsId { get; set; }
        public int TagsId { get; set; }

        public Problem Problem { get; set; }

       
        public Tag Tag { get; set; }

    }
}
