using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DTO.ProblemsDTO
{
    /// <summary>
    /// DTO for fetching a problem from an external source to associate with a contest.
    /// </summary>
    public class FetchProblemDto
    {
        //[Required]
        //public string ProblemSource { get; set; }
        //[Required]
        //public string ProblemLink { get; set; }
        //[Required]
        //public string ProblemId { get; set; }
        //[Required]
        //public int ContestID { get; set; }


        /// <summary>
        /// The name of the external problem source (e.g., Codeforces, AtCoder).
        /// </summary>
        [Required]
        public string ProblemSource { get; set; }

        /// <summary>
        /// The full link/URL to the external problem.
        /// </summary>
        [Required]
        public string ProblemLink { get; set; }

        /// <summary>
        /// The unique identifier of the problem from the external source.
        /// </summary>
        [Required]
        public string ProblemId { get; set; }

        /// <summary>
        /// The ID of the contest this problem should be linked to.
        /// </summary>
        [Required]
        public int ContestID { get; set; }
    }
}
