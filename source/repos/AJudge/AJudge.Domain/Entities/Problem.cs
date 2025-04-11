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
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public string[] SampleInput { get; set; }
        public string[] SampleOutput { get; set; }
        public int Rating { get; set; }

        // Navigation Properties
        [ForeignKey("ContestId")]
        Contest Contest { get; set; }
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
