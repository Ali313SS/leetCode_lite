using AJudge.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace AJudge.Application.DtO.CommentDTO
{
    /// <summary>
    /// Data transfer object used for updating an existing comment.
    /// </summary>
    public class UpdateCommentDTO
    {
        /// <summary>
        ///  sets the updated content of the comment.
        /// This field is required.
        /// </summary>
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
