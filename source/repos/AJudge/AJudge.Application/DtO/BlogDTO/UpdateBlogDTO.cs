using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO.BlogDTO
{
    public class UpdateBlogDTO
    {
        [Required]
        public string Content { get; set; }
      
        public static  Blog  ConvertToBlog(UpdateBlogDTO request)
        {
            return new Blog
            {
                Content = request.Content,
              
            };
        }

    }
}
