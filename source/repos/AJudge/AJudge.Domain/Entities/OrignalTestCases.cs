using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class OrignalTestCases
    {
        
        public int Id { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string ProblemId { get; set; }
        [ForeignKey("ProblemId")]
        public OrignalProblems Problem { get; set; }



    }
}
