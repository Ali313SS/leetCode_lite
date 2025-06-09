using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class UpdateContestRequest
    {
        public string Name { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
       // public List<Problem> Problems { get; set; }
    }
}
