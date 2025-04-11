using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Statistics
    {
        [Key]
        public int StatsId { get; set; }
        public int UserId { get; set; }
        public string PerformanceData { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
