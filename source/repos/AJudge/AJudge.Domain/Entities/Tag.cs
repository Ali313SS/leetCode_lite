using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }
        

        // Navigation Properties       
        public ICollection<Problem> Problems { get; set; } = new List<Problem>();
        public ICollection<ProblemTag> ProblemTags { get; set; } = new List<ProblemTag>();

    }
}
