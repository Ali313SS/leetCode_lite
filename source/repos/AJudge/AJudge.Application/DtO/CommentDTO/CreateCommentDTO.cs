using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO.CommentDTO
{
    public class CreateCommentDTO
    {
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }
        [Required]
        public int  BlogId { get; set; }
       // [Required]
      //  public int UserId { get; set; }

        public static Comment ConvertToComment(CreateCommentDTO request)
        {
            return new Comment
            {
                Content = request.Content,
                BlogId = request.BlogId,
               // UserId = request.UserId
            };
        }
    }

}
