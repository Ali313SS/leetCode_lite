using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Blog
    {
        [Key]
        public int BlogId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AuthorUserId { get; set; }        

        // Navigation Properties
        [ForeignKey("AuthorUserId")]
        public User Author { get; set; }
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
