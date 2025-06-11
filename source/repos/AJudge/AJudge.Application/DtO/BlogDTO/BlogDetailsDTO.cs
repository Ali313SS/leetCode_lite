using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO.BlogDTO
{
    /// <summary>
    /// Data transfer object representing detailed information about a blog post.
    /// </summary>
    public class BlogDetailsDTO
    {
       


        /// <summary>
        /// Gets or sets the content of the blog post.
        /// </summary>
        /// 
        public int BlogId { get; set; }
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the blog post was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the name of the author of the blog post.
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// Gets or sets the number of upvotes the blog post has received.
        /// </summary>
        public int UpVotesNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of downvotes the blog post has received.
        /// </summary>
        public int DownVotesNumber { get; set; }


        public static  BlogDetailsDTO ConvertToBlogDTO(Blog? blog)
        {
            return new BlogDetailsDTO
            {

                BlogId = blog.BlogId,
                Content = blog.Content,
                CreatedAt = blog.CreatedAt,
                AuthorName = blog.Author?.Username,
                UpVotesNumber = blog.Votes.Where(x => x.VoteType == VoteType.Upvote).Count(),
                DownVotesNumber = blog.Votes.Where(x => x.VoteType == VoteType.Downvote).Count()
            };
        }
    }

 
}
