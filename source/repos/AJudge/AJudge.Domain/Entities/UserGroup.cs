using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{

    [NotMapped]
    public class UserGroup
    {
        public int UserGroupId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public DateTime JoinedAt { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Group Group { get; set; }
    }
}
