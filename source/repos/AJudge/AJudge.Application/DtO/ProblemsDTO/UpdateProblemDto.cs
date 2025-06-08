using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Application.DtO.CommentDTO;
using AJudge.Domain.Entities;

namespace AJudge.Application.DtO.ProblemsDTO
{
    public class UpdateProblemDto
    {
        [Required]
        public string Description { get; set; }
        public int  numberOfTestCases { get; set; }
        public static Problem ConvertToProblem(UpdateProblemDto request)
        {
            return new Problem
            {
                Description = request.Description,
                 numberOfTestCases=request.numberOfTestCases
            };
        }
    }

}

