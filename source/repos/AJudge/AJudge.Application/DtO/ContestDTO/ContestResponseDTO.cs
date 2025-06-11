using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Application.DTO.UserDTOS;
using AJudge.Domain.Entities;

namespace AJudge.Application.DtO.ContestDTO
{
    public class ContestResponseDTO
    {
        public string Name { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public string Tutorial { get; set; }
        public static ContestResponseDTO ConvertToContestResponse(Contest contest)
        {
            return new ContestResponseDTO
            {
               Name=contest.Name,
               BeginTime = contest.BeginTime,
               EndTime = contest.EndTime,
               Status = contest.Status,
               Tutorial = contest.Tutorial,


            };
        }
    }
}
