using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Manager
    {
        public int ManagerId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Group Group { get; set; }
        public ICollection<ManagerPermission> ManagerPermissions { get; set; } = new List<ManagerPermission>();
    }
}
