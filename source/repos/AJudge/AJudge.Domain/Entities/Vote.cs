using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class Vote
    {
        public int VoteId { get; set; }
        public VoteType VoteType { get; set; }
        public int UserId { get; set; }
        public int? BlogId { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public User Voter { get; set; }
        [ForeignKey("BlogId")]
        public Blog Blog { get; set; }



        public Guid? CommentId { get; set; }
        [ForeignKey(nameof(Vote.CommentId))]
        public Comment Comment { get; set; }
    }

    public enum VoteType
    {
        Upvote,
        Downvote
    }
}
