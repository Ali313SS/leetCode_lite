using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public interface IUserService
    {
        Task<UserPagination> GetAllUserInClubPerPage(string filterBy, string filterValue, string sortBy, bool isAsinding = true, int pageNumber = 1, int pageSize = 20);
       
    }
}
