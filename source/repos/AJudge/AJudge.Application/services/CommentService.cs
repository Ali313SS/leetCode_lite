using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public class CommentService:ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CommentService(IUnitOfWork unitofWork)
        {
            _unitOfWork = unitofWork;
        }

        public async Task<Pagination<Comment>> GetAllCommentInPage(string sortBy, bool isAsinding = true, int pageNumber = 1, int pageSize = 20)
        {
            IQueryable<Comment> query = _unitOfWork.Comment.GetQuery();
            query = _unitOfWork.Comment.ApplySort(query, sortBy, isAsinding);

            Pagination<Comment> commentPage = await Pagination<Comment>.GetPageDetails(query, 1, 10);
            return commentPage;
        }

      
    }
}
