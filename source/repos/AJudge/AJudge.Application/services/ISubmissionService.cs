using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJudge.Domain.Entities;

namespace AJudge.Application.services
{
    public interface ISubmissionService
    {
        Task<SubmissionPagination> GetAllSubmissionInJudgePage(string? filterBy, string? filterValue, int pageNumber = 1, int pageSize = 20);
        Task<SubmissionPagination> GetUserSubmissionsAsync(int userId, string? onlineJudge, int pageNumber, int pageSize);
        Task<SubmissionPagination> GetFollowedUsersSubmissionsAsync(int userId, string? groupType, string? onlineJudge, int pageNumber, int pageSize);

    }

    public class SubmissionPagination
    {
        public Submission[] Items { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
