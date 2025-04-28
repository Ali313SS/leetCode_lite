using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class UserTeamInvitation
    {
        public int UserId { get; set; }
        public int TeamId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; } = null!;
    }
}
