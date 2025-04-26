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
    public class ProblemPageDTO
    {
        public List<ProblemDTO> items { get; set; }
        public int TotalPages  { get; set; }
        public int PageNumber { get; set; }
        public bool HasNext { get; set; }
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
