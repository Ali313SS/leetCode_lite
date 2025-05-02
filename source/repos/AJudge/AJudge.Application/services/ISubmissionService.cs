using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public interface ISubmissionService
    {
        Task<SubmissionPagination> GetAllSubmissionInJudgePage(string? filterBy, string? filterValue, int pageNumber = 1, int pageSize = 20);

    }
}
