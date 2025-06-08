using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Application.DtO.CommentDTO;
using AJudge.Domain.Entities;

namespace AJudge.Application.DtO.ProblemsDTO
{
    public class ProblemResponseDto
    {

        public string Description { get; set; }
        public int numberOfTestCases { get; set; }

        public static ProblemResponseDto ConvertToProblemResponseDTO(Problem prob)
        {
            return new ProblemResponseDto
            {
                Description = prob.Description,
                numberOfTestCases = prob.numberOfTestCases,
            };
        }
    }
}
