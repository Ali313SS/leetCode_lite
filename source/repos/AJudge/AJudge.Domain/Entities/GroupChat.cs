using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class GroupChat
    {
        public int ChatId { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }

        // Navigation Properties
        public Group Group { get; set; }
    }
}
