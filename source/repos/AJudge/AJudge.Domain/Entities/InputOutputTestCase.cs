using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
   public class InputOutputTestCase
    {
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public int TestCaseId { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        [ForeignKey("ProblemId")]
        public Problem Problem { get; set; }
        
    }
}
