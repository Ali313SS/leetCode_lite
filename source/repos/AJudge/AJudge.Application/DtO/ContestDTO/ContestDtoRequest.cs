using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Application.DTO.GroupDTO;
using AJudge.Domain.Entities;

namespace AJudge.Application.DtO.ContestDTO
{
    public class CProblems
    {

        public int Id { get; set; }

        public string Name { get; set; }

        
        public DateTime Created { get; set; }

        public DateTime Ended { get; set; }

    }
    public class ContestDtoRequest
    {


        public int ContestId { get; set; }

        /// <summary>
        /// Gets or sets the name of the contest.
        /// </summary>
        public string Name { get; set; }
        public string Tutorial { get; set; }
        public string Status { get; set; }
         public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of the contest.
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// Gets or sets the end date and time of the contest.
        /// </summary>
        public DateTime EndTime { get; set; }
        public ICollection<CProblems> Problems { get; set; } = new List<CProblems>();

    }
}
