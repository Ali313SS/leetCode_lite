using AJudge.Application.DtO.ProblemsDTO;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Application.Services;
using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public class ProblemService : IProblemService
    {
        private IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public ProblemService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<ProblemDetailsDTO?> GetProblemDetailsAsync(int problemId, int? userId)
        {
            Problem? problem = await _unitOfWork.Problem.GetSpecific(x => x.ProblemId == problemId, new[] { "Tags", "Submissions", "TestCases" });
            if (problem == null)
                return null;

            string state = userId == null ? "Unattempted" : problem.Submissions
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.SubmittedAt)
                .Select(x => x.Result)
                .FirstOrDefault() ?? "Unattempted";

            List<string>? tagsName = problem.Tags.Select(x => x.Name).ToList();

            var testCases = problem.TestCases.Select(x => new InputOutputTestCasesDTO
            {
                Input = x.Input,
                Output = x.Output
            }).ToList();

            ProblemDetailsDTO problemDetailsDTO = ProblemDetailsDTO.ConvertToProblemDetalsDTO(problem, state, tagsName, testCases);
            return problemDetailsDTO;
        }

        public Task<OrignalProblems> FetchProblem(FetchProblemDto problemDTO)
        {
            var getSoruceID = _context.Sources.FirstOrDefault(x => x.SourceName == problemDTO.ProblemSource)?.SourceId;

            if (getSoruceID == null)
                return null;

            var problemdId = getSoruceID.ToString().Length.ToString() + getSoruceID + problemDTO.ProblemId;
            var problemexist = _context.OrignalProblems.Include(x => x.TestCases).FirstOrDefault(x => x.ProblemId == problemdId);

            if (problemexist != null)
            {
                return Task.FromResult(problemexist);
            }

            IFetchServices Fetch = FactoryFetch.GetFactory(problemDTO.ProblemSource);
            var problem = Fetch.FetchFrom(problemDTO.ProblemLink).Result;

            if (problem == null)
                return null;

            var SaveProblem = new OrignalProblems
            {
                ProblemId = getSoruceID.ToString().Length.ToString() + getSoruceID + problemDTO.ProblemId,
                ProblemName = problem.ProblemName,
                Description = problem.Description,
                InputFormat = problem.InputFormat,
                OutputFormat = problem.OutputFormat,
                ProblemSource = problem.ProblemSource,
                ProblemLink = problem.ProblemLink,
                numberOfTestCases = problem.numberOfTestCases,
                Rating = problem.Rating,
                ProblemSourceID = (int)getSoruceID,
            };

            problem.TestCases.ToList().ForEach(x =>
            {
                var testCase = new OrignalTestCases
                {
                    Input = x.Input,
                    Output = x.Output
                };
                SaveProblem.TestCases.Add(testCase);
            });

            _context.OrignalProblems.Add(SaveProblem);
            _context.SaveChangesAsync();
            return Task.FromResult(SaveProblem);
        }

        public Task<bool> ChangeProblemStatement(ProblemDTO problemDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProblem(int problemId)
        {
            throw new NotImplementedException();
        }

        public Task<ProblemDTO> GetProblem(int problemId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProblemDTO>> GetProblemsByDifficulty(string difficulty)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProblemDTO>> GetProblemsBySource(string source)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProblemDTO>> GetProblemsByTag(string tag)
        {
            throw new NotImplementedException();
        }

        public async Task<ProblemDetailsDTO?> GetProblemByName(string name, int? userId)
        {
            Problem? problem = await _unitOfWork.Problem.GetSpecific(x => x.ProblemName == name, new[] { "Tags", "Submissions", "TestCases" });
            if (problem == null)
                return null;

            string state = userId == null ? "Unattempted" : problem.Submissions
              .Where(x => x.UserId == userId)
              .OrderByDescending(x => x.SubmittedAt)
              .Select(x => x.Result)
              .FirstOrDefault();

            List<string>? tagsName = problem.Tags.Select(x => x.Name).ToList();

            var testCases = problem.TestCases.Select(x => new InputOutputTestCasesDTO
            {
                Input = x.Input,
                Output = x.Output
            }).ToList();

            ProblemDetailsDTO problemDEtailsDTO = ProblemDetailsDTO.ConvertToProblemDetalsDTO(problem, state, tagsName, testCases);
            return problemDEtailsDTO;
        }

        public async Task<List<Problem>> GetAllProblems(int problemId)
        {
            List<Problem> problems = (List<Problem>)await _unitOfWork.Problem.GetAll();
            return problems;
        }

        public async Task<ProblemPagination> GetAllProblemsInPage(string sortBy, bool isAsinding = true, int pageNumber = 1, int pageSize = 100)
        {
            IQueryable<Problem> query = _unitOfWork.Problem.GetQuery();
            query = BuildSort(query, sortBy, isAsinding);
            ProblemPagination probemsPage = await ProblemPagination.GetPageDetails(query, pageNumber, pageSize);
            return probemsPage;
        }

        private IQueryable<Problem> BuildSort(IQueryable<Problem> query, string? sortBy, bool isAssending = true)
        {
            if (!string.IsNullOrEmpty(sortBy))
            {
                var parameter = Expression.Parameter(typeof(Problem), "x");
                var property = Expression.Property(parameter, sortBy);
                var propertyType = property.Type;
                var lambda = Expression.Lambda(property, parameter);
                string methodName = isAssending ? "OrderBy" : "OrderByDescending";
                var result = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { typeof(Problem), propertyType },
                    query.Expression,
                    Expression.Quote(lambda)
                );
                query = query.Provider.CreateQuery<Problem>(result);
            }
            return query;
        }

        public Task<string> SubmitProblem(int userid, string link,string Source,  string code,string lang)

        {
            
            IFetchServices Fetch = FactoryFetch.GetFactory(Source);            
            var submission = Fetch.Submit(link, code, lang, 1).Result;
            return Task.FromResult(submission);
        }
    }

    public class ProblemPagination
    {
        public List<Problem> Items { get; private set; }
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        public ProblemPagination(List<Problem> items, int count, int pageNumber, int pageSize)
        {
            Items = items ?? new List<Problem>();
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public static async Task<ProblemPagination> GetPageDetails(IQueryable<Problem> source, int pageNumber = 1, int pageSize = 100)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new ProblemPagination(items, count, pageNumber, pageSize);
        }
    }
}