using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Domain.Entities;

namespace AJudge.Application.DtO.ContestDTO
{
    public class ContestDto
    {

        public int ContestId { get; set; }
        public string Name { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public static ContestDto ConvertToContestDto(Contest contest)
        {
            return new ContestDto
            {
ContestId = contest.ContestId,
Name = contest.Name,
BeginTime = contest.BeginTime,
EndTime = contest.EndTime,
            };

        }

    }
}
