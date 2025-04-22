using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DTO.ProblemsDTO
{
   public class ProblemDTO
    {
        public string ProblemSource { get; set; }
        public string ProblemName { get; set; }
        public int Rating { get; set; }
        //public string? ProblemId { get; set; }
        [AllowNull]
        public string problemLink { get; set; }


        public static ProblemDTO ConvertToProblemDTO(Problem  problem)
        {
            return new ProblemDTO
            {
                ProblemSource = problem.ProblemSource,
                problemLink = problem.ProblemLink,
                ProblemName = problem.ProblemName,
                Rating = problem.Rating
            };

        }
    }
}
