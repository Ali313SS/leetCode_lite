using AJudge.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace AJudge.Application.DtO.BlogDTO
{
    /// <summary>
    /// Data transfer object used for updating an existing blog post.
    /// </summary>
    public class UpdateBlogDTO
    {
        /// <summary>
        /// Gets or sets the updated content of the blog post.
        /// This field is required.
        /// </summary>
        [Required]
        public string Content { get; set; }

        public static  Blog  ConvertToComment(UpdateBlogDTO request)
        {
            return new Blog
            {
                Content = request.Content,
              
            };
        }

    }
}
