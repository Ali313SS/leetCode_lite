using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AJudge.Application.services
{
    public class SubmissionService : ISubmissionService
    {

        private readonly IUnitOfWork _unitOfWork;
        public SubmissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }

       public async Task<SubmissionPagination> GetAllSubmissionInJudgePage(string? filterBy, string? filterValue, int pageNumber, int pageSize)
        { 

            var submissions = await _unitOfWork.Submission.GetAllApply(x => x.Group.Privacy == PrivacyType.Public,
                new[] { nameof(Submission.Group), nameof(Submission.User), nameof(Submission.Problem) });


               if (filterBy != null && filterValue != null)
               {
                    submissions = submissions.Where(x => x.Problem.ProblemSource.ToLower() == filterValue.ToLower()).ToList();
               }

            var page = SubmissionPagination.GetPage(submissions, pageNumber, pageSize);
            return page;

        }


    }
    public class SubmissionPagination
    {
        public List<Submission> Items { get; set; }
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        public SubmissionPagination(List<Submission> items, int count, int pageNumber, int pageSize)
        {
            Items = items ?? new List<Submission>();
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }



        public static  SubmissionPagination GetPage(List<Submission> list, int pageNumber = 1, int pageSize = 20)
        {
            var count = list.Count();
            var items = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            
            return new SubmissionPagination(items, count, pageNumber, pageSize);
        }



        
    }

}
