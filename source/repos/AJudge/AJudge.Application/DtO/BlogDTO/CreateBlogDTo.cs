using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO.BlogDTO
{
    /// <summary>
    /// Data transfer object used for creating a new blog post.
    /// </summary>
    public class CreateBlogDTo
    {
       


        /// <summary>
        /// Gets or sets the content of the blog post.
        /// This field is required.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Gets the date and time when the blog post is created.
        /// This property is automatically set to the current date and time.
        /// </summary>
        public DateTime CreatedAt => DateTime.Now;


        public static  Blog  ConvertToBlog(CreateBlogDTo request)
        {
            return new Blog
            {
                Content = request.Content,
                CreatedAt = request.CreatedAt,
            };
        }

    }
}
