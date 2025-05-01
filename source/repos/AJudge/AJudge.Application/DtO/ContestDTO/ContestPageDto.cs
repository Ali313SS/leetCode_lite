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
    public class ContestPageDto
    {

        public List<ContestDto> items { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public bool HasNext { get; set; }
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
