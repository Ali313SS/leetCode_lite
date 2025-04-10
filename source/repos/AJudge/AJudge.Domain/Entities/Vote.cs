using System;
using System.Collections.Generic;
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
        public int BlogId { get; set; }

        // Navigation Properties
        public User Voter { get; set; }
        public Blog Blog { get; set; }
    }

    public enum VoteType
    {
        Upvote,
        Downvote
    }
}
