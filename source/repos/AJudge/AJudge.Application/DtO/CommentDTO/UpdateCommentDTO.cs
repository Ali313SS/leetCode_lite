using AJudge.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace AJudge.Application.DtO.CommentDTO
{
    public class UpdateCommentDTO
    {
        [Required]
        public string Content { get; set; }

        public static Comment ConvertToComment(UpdateCommentDTO request)
        {
            return new Comment
            {
                Content = request.Content,

            };
        }
    }
}
