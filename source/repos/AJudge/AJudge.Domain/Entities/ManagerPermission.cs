using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class ManagerPermission
    {
        public int ManagerPermissionId { get; set; }
        public int ManagerId { get; set; }
        public int PermissionId { get; set; }

        // Navigation Properties
        public Manager Manager { get; set; }
        public Permission Permission { get; set; }
    }
}
