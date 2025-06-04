using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Domain.Entities;

namespace AJudge.Application.DtO.ContestDTO
{
    /// <summary>
    /// Data transfer object representing a contest.
    /// </summary>
    public class ContestDto
    {

        


        /// <summary>
        /// Gets or sets the unique identifier of the contest.
        /// </summary>
        public int ContestId { get; set; }

        /// <summary>
        /// Gets or sets the name of the contest.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of the contest.
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// Gets or sets the end date and time of the contest.
        /// </summary>
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
