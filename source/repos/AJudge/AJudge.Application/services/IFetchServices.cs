using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
   public interface IFetchServices
    {
        public Task<Problem> FetchFrom(string url);
        public Task<Submission>Submit(string url, string code, string language, int problemId);

    }


}
