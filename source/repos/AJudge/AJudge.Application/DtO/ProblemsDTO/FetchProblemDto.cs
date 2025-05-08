using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DTO.ProblemsDTO
{
    public class FetchProblemDto
    {
        [Required]
        public string ProblemSource { get; set; }
        [Required]
        public string ProblemLink { get; set; }
        [Required]
        public string ProblemId { get; set; }
        [Required]
        public int ContestID { get; set; }
    }
}
