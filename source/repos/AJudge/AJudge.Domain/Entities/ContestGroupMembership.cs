using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class ContestGroupMembership
    {

        public int ContestId { get; set; }
        public Contest Contest { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
