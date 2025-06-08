using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DTO.ProblemsDTO
{
    /// <summary>
    /// Data Transfer Object representing a programming problem.
    /// </summary>
    public class ProblemDTO
    {
        /// <summary>
        /// The source of the problem (e.g., Codeforces, LeetCode).
        /// </summary>
        public string ProblemSource { get; set; }
        /// <summary>
        /// The external ID of the problem from its source platform, if available.
        /// </summary>
        public int? ProblemSourceID { get; set; }

        /// <summary>
        /// The name/title of the problem.
        /// </summary>
        public string ProblemName { get; set; }
        /// <summary>
        /// The difficulty rating of the problem.
        /// </summary>
        public int Rating { get; set; }
        /// <summary>
        /// The unique internal ID of the problem.
        /// </summary>
        public int? ProblemId { get; set; }

        /// <summary>
        /// A link to the problem (e.g., external URL).
        /// </summary>
        [AllowNull]
        public string problemLink { get; set; }
        /// <summary>
        /// The ID of the contest this problem is part of.
        /// </summary>
        public int ContestId { get; set; }
        /// <summary>
        /// The number of test cases associated with this problem.
        /// </summary>
        public int NumberOfTestCases { get; set; }

        public static ProblemDTO ConvertToProblemDTO(Problem problem)
        {
            return new ProblemDTO
            {
                ContestId = problem.ContestId,
                ProblemSourceID = problem.ProblemSourceID,

                NumberOfTestCases = problem.numberOfTestCases,
                ProblemSource = problem.ProblemSource,
                problemLink = problem.ProblemLink,
                ProblemName = problem.ProblemName,
                Rating = problem.Rating,
                ProblemId=problem.ProblemId
            };

        }

    
    }
}