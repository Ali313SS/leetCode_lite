using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public interface ICommentService
    {
        Task<Pagination<Comment>> GetAllCommentInPage(Expression<Func<Comment, bool>> pred, string sortBy, bool isAsinding = true, int pageNumber = 1, int pageSize = 20);

    }
}
