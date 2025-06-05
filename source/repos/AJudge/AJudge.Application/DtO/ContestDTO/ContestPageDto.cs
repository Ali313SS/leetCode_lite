using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Application.DtO.ProblemsDTO;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Application.services;
using AJudge.Domain.Entities;

namespace AJudge.Application.DtO.ContestDTO
{
    /// <summary>
    /// Data transfer object representing a paginated list of contests.
    /// </summary>
    public class ContestPageDto
    {

      



        /// <summary>
        /// Gets or sets the list of contests on the current page.
        /// </summary>
        public List<ContestDto> items { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages available.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the current page number (1-based).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there is a next page available.
        /// </summary>
        public bool HasNext { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there is a previous page available.
        /// </summary>
        public bool HasPrevious { get; set; }
        public static ContestPageDto ConvertToContestDto(ContestPagination pagination)
        {
            return new ContestPageDto
            {
                items = pagination.Items.Select(x => new ContestDto
                {

                    ContestId =x.ContestId,
                    Name = x.Name,
                    BeginTime = x.BeginTime,
                    EndTime = x.EndTime,
                    
                }).ToList(),
                TotalPages = pagination.TotalPages,
                PageNumber = pagination.PageNumber,
                HasNext = pagination.HasNext,
                HasPrevious = pagination.HasPrevious,
            };
        }


    }
}
