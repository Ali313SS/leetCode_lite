using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO.BlogDTO
{
    public class CreateBlogDTo
    {
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt =>DateTime.Now;
        [Required]
        public int CreaterId { get; set; }

        public static  Blog  ConvertToBlog(CreateBlogDTo request)
        {
            return new Blog
            {
                Content = request.Content,
                CreatedAt = request.CreatedAt,
                AuthorUserId = request.CreaterId,
            };
        }

    }
}
