using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DTO.ProblemsDTO
{
   public class ProblemDTO
    {
        public int ProblemId { get; set; }
        public string ProblemName { get; set; }
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public int Rating { get; set; }
        public string ProblemLink { get; set; }
        public string ProblemSource { get; set; }
        public string ProblemSourceID { get; set; }
        public int numberOfTestCases { get; set; }



        public static ProblemDTO ConvertToProblemDTO(Problem  problem)
        {
            return new ProblemDTO
            {
                ProblemId = problem.ProblemId,
                ProblemName = problem.ProblemName,
                Description = problem.Description,
                InputFormat = problem.InputFormat,
                OutputFormat = problem.OutputFormat,
                Rating = problem.Rating,
                ProblemLink = problem.ProblemLink,
                ProblemSource = problem.ProblemSource,
                ProblemSourceID = problem.ProblemSourceID,
                numberOfTestCases = problem.numberOfTestCases
            };

        }

        public class ProblemDetailsDTO : ProblemDTO
        {
            public List<int> TagIds { get; set; } = new List<int>();
            public List<string> TestCaseInputs { get; set; } = new List<string>();
            public List<string> TestCaseOutputs { get; set; } = new List<string>();
        }
    }
}
