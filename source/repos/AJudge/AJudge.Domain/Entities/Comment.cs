using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public int BlogId { get; set; }
        [ForeignKey(nameof(Comment.BlogId))]
        public Blog Blog { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(Comment.UserId))]
        public User User { get; set; }

        public ICollection<Vote> Votes { get; set; } = new List<Vote>();



    }
}
