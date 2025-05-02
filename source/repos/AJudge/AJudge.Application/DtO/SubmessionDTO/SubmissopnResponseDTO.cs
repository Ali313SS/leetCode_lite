using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO.SubmessionDTO
{
    public class SubmissopnResponseDTO
    {
        public string? UserName { get; set; }
        public string ?GroupName { get; set; }
        public string? Judge { get; set; }
        public string ?ProblemName { get; set; }
        public string Result { get; set; }
        public DateTime SubmissionAt { get; set; }

        public static SubmissopnResponseDTO ConvertToSubmessionResponse(Submission submession)
        {
            return new SubmissopnResponseDTO
            {
                UserName = submession?.User.Username,
                Judge = submession?.Problem.ProblemSource,
                ProblemName=submession?.Problem.ProblemName,
                GroupName=submession?.Group.Name,
                Result = submession.Result,
                SubmissionAt = submession.SubmittedAt,

            };
        }
    }
}
