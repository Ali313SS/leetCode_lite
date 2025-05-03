using Microsoft.EntityFrameworkCore;

namespace AJudge.Application.services
{
    public class Pagination<T>
    {
        public List<T> Items { get; set; }
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        public Pagination(List<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items ?? new List<T>();
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public static async Task<Pagination<T>> GetPageDetails(IQueryable<T> source, int pageNumber = 1, int pageSize = 20)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new Pagination<T>(items, count, pageNumber, pageSize);
        }
    }
}
