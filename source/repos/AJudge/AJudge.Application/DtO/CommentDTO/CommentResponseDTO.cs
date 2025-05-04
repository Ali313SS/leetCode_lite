using AJudge.Domain.Entities;

namespace AJudge.Application.DtO.CommentDTO
{
    public class CommentResponseDTO
    {
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? BlogId { get; set; }
        public int UserId { get; set; }
        public int UpVotes { get; set; }
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
