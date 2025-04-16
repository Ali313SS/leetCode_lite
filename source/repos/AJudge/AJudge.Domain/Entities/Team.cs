using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Team
    {
        
            public int TeamId { get; set; }
            public string Name { get; set; }
            public DateTime CreatedAt { get; set; }
      //  public int UserTeams { get; set; }
        // Navigation Properties
         public ICollection<User> Users { get; set; } = new List<User>();

        public ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();

    }
}
