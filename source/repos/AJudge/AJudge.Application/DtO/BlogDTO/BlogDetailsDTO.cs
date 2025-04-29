using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO.BlogDTO
{
    public class BlogDetailsDTO
    {
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; }
        public int UpVotesNumber { get; set; }
        public int DownVotesNumber { get; set; }


        public static  BlogDetailsDTO ConvertToBlogDTO(Blog? blog)
        {
            return new BlogDetailsDTO
            {
                Content = blog.Content,
                CreatedAt = blog.CreatedAt,
                AuthorName = blog.Author?.Username,
                UpVotesNumber = blog.Votes.Where(x => x.VoteType == VoteType.Upvote).Count(),
                DownVotesNumber = blog.Votes.Where(x => x.VoteType == VoteType.Downvote).Count()
            };
        }
    }

 
}
