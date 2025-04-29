using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class CoachRequest
    {

        public int UserId { get; set; }
        public int CoachId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        [ForeignKey("CoachId")]
        public virtual User? Coach { get; set; }

    }
}
