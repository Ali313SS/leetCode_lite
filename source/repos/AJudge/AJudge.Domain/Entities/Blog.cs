using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AuthorUserId { get; set; }

        // Navigation Properties
        public User Author { get; set; }
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
