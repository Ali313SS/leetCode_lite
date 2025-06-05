//using AJudge.Domain.Entities;
//using AJudge.Domain.RepoContracts;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

//namespace AJudge.Application.services
//{
//    public class SubmissionService : ISubmissionService
//    {

//        private readonly IUnitOfWork _unitOfWork;
//        public SubmissionService(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<SubmissionPagination> GetAllSubmissionInJudgePage(string? filterBy, string? filterValue, int pageNumber, int pageSize)
//        {

//            var submissions = await _unitOfWork.Submission.GetAllApply(x => x.Group.Privacy == PrivacyType.Public,
//                new[] { nameof(Submission.Group), nameof(Submission.User), nameof(Submission.Problem) });


//            if (filterBy != null && filterValue != null)
//            {
//                submissions = submissions.Where(x => x.Problem.ProblemSource.ToLower() == filterValue.ToLower()).ToList();
//            }

//            var page = SubmissionPagination.GetPage(submissions, pageNumber, pageSize);
//            return page;

//        }


//    }
//    public class SubmissionPagination
//    {
//        public List<Submission> Items { get; set; }
//        public int PageNumber { get; private set; }
//        public int TotalPages { get; private set; }
//        public bool HasPrevious => PageNumber > 1;
//        public bool HasNext => PageNumber < TotalPages;

//        public SubmissionPagination(List<Submission> items, int count, int pageNumber, int pageSize)
//        {
//            Items = items ?? new List<Submission>();
//            PageNumber = pageNumber;
//            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
//        }



//        public static SubmissionPagination GetPage(List<Submission> list, int pageNumber = 1, int pageSize = 20)
//        {
//            var count = list.Count();
//            var items = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


//            return new SubmissionPagination(items, count, pageNumber, pageSize);
//        }




//    }

//}

using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubmissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }


        public async Task<SubmissionPagination> GetAllSubmissionInJudgePage(string judgeProperty, string? onlineJudge, int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("PageNumber and PageSize must be greater than 0.");
            }

            var query = _unitOfWork.Submission.GetQuery()
                .Include(s => s.User)
                .Include(s => s.Problem)
                .Include(s => s.Group)
                .Where(s => s.Group != null && s.Group.Privacy == PrivacyType.Public);

            if (!string.IsNullOrEmpty(onlineJudge))
            {
                query = query.Where(s => s.Problem != null && s.Problem.ProblemSource == onlineJudge);
            }

            var totalCount = await query.CountAsync();
            if (totalCount == 0)
            {
                return new SubmissionPagination
                {
                    Items = Array.Empty<Submission>(),
                    PageNumber = pageNumber,
                    TotalPages = 0,
                    HasPrevious = false,
                    HasNext = false
                };
            }

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .OrderByDescending(s => s.SubmittedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            return new SubmissionPagination
            {
                Items = items,
                PageNumber = pageNumber,
                TotalPages = totalPages,
                HasPrevious = pageNumber > 1,
                HasNext = pageNumber < totalPages
            };
        }


        public async Task<SubmissionPagination> GetUserSubmissionsAsync(int userId, string? onlineJudge, int pageNumber, int pageSize)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("PageNumber and PageSize must be greater than 0.");
            }

            var query = _unitOfWork.Submission.GetQuery()
                .Include(s => s.User)
                .Include(s => s.Problem)
                .Include(s => s.Group)
                .Where(s => s.UserId == userId);

            if (!string.IsNullOrEmpty(onlineJudge))
            {
                query = query.Where(s => s.Problem != null && s.Problem.ProblemSource == onlineJudge);
            }

            var totalCount = await query.CountAsync();
            if (totalCount == 0)
            {
                return new SubmissionPagination
                {
                    Items = Array.Empty<Submission>(),
                    PageNumber = pageNumber,
                    TotalPages = 0,
                    HasPrevious = false,
                    HasNext = false
                };
            }

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .OrderByDescending(s => s.SubmittedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            return new SubmissionPagination
            {
                Items = items,
                PageNumber = pageNumber,
                TotalPages = totalPages,
                HasPrevious = pageNumber > 1,
                HasNext = pageNumber < totalPages
            };
        }
      
        public async Task<SubmissionPagination> GetFollowedUsersSubmissionsAsync(int userId, string? groupType, string? onlineJudge, int pageNumber, int pageSize)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("PageNumber and PageSize must be greater than 0.");
            }

            var followedUserIds = await _unitOfWork.UserFriends.GetQuery()
                .Where(uf => uf.UserId == userId)
                .Select(uf => uf.FriendId)
                .ToListAsync();

            if (!followedUserIds.Any())
            {
                return new SubmissionPagination
                {
                    Items = Array.Empty<Submission>(),
                    PageNumber = pageNumber,
                    TotalPages = 0,
                    HasPrevious = false,
                    HasNext = false
                };
            }

            var query = _unitOfWork.Submission.GetQuery()
                .Include(s => s.User)
                .Include(s => s.Problem)
                .Include(s => s.Group)
                .Where(s => followedUserIds.Contains(s.UserId));

            if (!string.IsNullOrEmpty(groupType))
            {
                if (groupType != "Public" && groupType != "Private")
                {
                    throw new ArgumentException("Invalid groupType. Must be 'Public' or 'Private'.");
                }
                var privacy = groupType == "Public" ? PrivacyType.Public : PrivacyType.Private;
                query = query.Where(s => s.Group != null && s.Group.Privacy == privacy);
            }

            if (!string.IsNullOrEmpty(onlineJudge))
            {
                query = query.Where(s => s.Problem != null && s.Problem.ProblemSource == onlineJudge);
            }

            var totalCount = await query.CountAsync();
            if (totalCount == 0)
            {
                return new SubmissionPagination
                {
                    Items = Array.Empty<Submission>(),
                    PageNumber = pageNumber,
                    TotalPages = 0,
                    HasPrevious = false,
                    HasNext = false
                };
            }

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .OrderByDescending(s => s.SubmittedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            return new SubmissionPagination
            {
                Items = items,
                PageNumber = pageNumber,
                TotalPages = totalPages,
                HasPrevious = pageNumber > 1,
                HasNext = pageNumber < totalPages
            };
        }
    }
}
