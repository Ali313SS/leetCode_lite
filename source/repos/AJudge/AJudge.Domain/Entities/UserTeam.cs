using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    [NotMapped]
    public class UserTeam
    {
        public int UserTeamId { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Team Team { get; set; }
    }
}
