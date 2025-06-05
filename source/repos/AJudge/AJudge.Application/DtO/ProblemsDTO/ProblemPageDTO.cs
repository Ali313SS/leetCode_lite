using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Application.services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO.ProblemsDTO
{
    /// <summary>
    /// Represents a paginated list of problems with paging metadata.
    /// </summary>
    public class ProblemPageDTO
    {
      

        /// <summary>
        /// List of problems on the current page.
        /// </summary>
        public List<ProblemDTO> items { get; set; }

        /// <summary>
        /// Total number of pages available.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Current page number (1-based).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Indicates if there is a next page available.
        /// </summary>
        public bool HasNext { get; set; }

        /// <summary>
        /// Indicates if there is a previous page available.
        /// </summary>
        public bool HasPrevious { get; set; }

        public static ProblemPageDTO ConvertToProblemPageDTO(ProblemPagination pagination)
        {
            return new ProblemPageDTO
            {
                items = pagination.Items.Select(x => new ProblemDTO
                {
                    ProblemSource = x.ProblemSource,
                    ProblemName = x.ProblemName,
                    Rating = x.Rating,
                    ProblemId = x.ProblemId,
                    NumberOfTestCases = x.numberOfTestCases,
                    problemLink = x.ProblemLink,
                    ContestId = x.ContestId
                }).ToList(),
                TotalPages= pagination.TotalPages,
                PageNumber= pagination.PageNumber,
                HasNext= pagination.HasNext,
                HasPrevious= pagination.HasPrevious,
            };
        }
    }
}
