using AJudge.Domain.Entities;

namespace AJudge.Application.DtO.CommentDTO
{

    /// <summary>
    /// Data transfer object representing the details of a comment response.
    /// </summary>
    public class CommentResponseDTO
    {
        /// <summary>
        /// Gets or sets the content of the comment.
        /// This property is nullable.
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the comment was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the associated blog post ID.
        /// This property is nullable.
        /// </summary>
        public int? BlogId { get; set; }

        /// <summary>
        /// Gets or sets the user ID of the comment author.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the number of upvotes the comment has received.
        /// </summary>
        public int UpVotes { get; set; }

        /// <summary>
        /// Gets or sets the number of downvotes the comment has received.
        /// </summary>
        public int DownVotes { get; set; }

        public static CommentResponseDTO ConvertToCommentResponseDTO(Comment comment)
        {
            return new CommentResponseDTO
            {
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                BlogId = comment.BlogId,
                UserId = comment.UserId,
                UpVotes = comment.Votes.Where(x => x.VoteType == VoteType.Upvote).Count(),
                DownVotes = comment.Votes.Where(x => x.VoteType == VoteType.Downvote).Count()
            };
        }
    }



}
