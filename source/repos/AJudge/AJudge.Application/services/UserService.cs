using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public class UserService:IUserService

    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitofWork)
        {
            _unitOfWork = unitofWork;   
        }


        public async Task<UserPagination> GetAllUserInClubPerPage(string filterBy,string filterValue,string sortBy, bool isAsinding = true, int pageNumber = 1, int pageSize =20)
        {

            IQueryable<User> query = _unitOfWork.User.GetQuery();



            query = _unitOfWork.User.ApplyFilter(query, filterBy, filterValue);
            query = _unitOfWork.User.ApplySort(query, sortBy, isAsinding);

            UserPagination userPage = await UserPagination.GetPageDetails(query, pageNumber, pageSize);

            return userPage;
        }
    }

    public class UserPagination
    {



        public List<User> Items { get;  set; }
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        public UserPagination(List<User> items, int count, int pageNumber, int pageSize)
        {
            Items = items ?? new List<User>();  
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public static async Task<UserPagination> GetPageDetails(IQueryable<User> source, int pageNumber = 1, int pageSize = 20)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new UserPagination(items, count, pageNumber, pageSize);
        }
    }
}
