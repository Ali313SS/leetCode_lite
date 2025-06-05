using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO.CommentDTO
{
    /// <summary>
    /// Data transfer object used for creating a new comment.
    /// </summary>
    public class CreateCommentDTO
    {

    /// <summary>
    /// Gets or sets the content of the comment.
    /// This field is required and has a maximum length of 1000 characters.
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; }

    /// <summary>
    /// Gets or sets the ID of the blog post to which this comment belongs.
    /// This field is required.
    /// </summary>
    [Required]
    public int BlogId { get; set; }

    public static Comment ConvertToComment(CreateCommentDTO request)
        {
            return new Comment
            {
                Content = request.Content,
                BlogId = request.BlogId,
            };
        }
    }

}
