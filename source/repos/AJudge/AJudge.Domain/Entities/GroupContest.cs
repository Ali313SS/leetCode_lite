using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class GroupContest
    {
        public int GroupContestId { get; set; }
        public int GroupId { get; set; }
        public int ContestId { get; set; }

        // Navigation Properties
        public Group Group { get; set; }
        public Contest Contest { get; set; }
    }
}
