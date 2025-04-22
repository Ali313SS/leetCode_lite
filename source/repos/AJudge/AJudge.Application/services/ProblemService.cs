//using AJudge.Application.DtO.ProblemsDTO;
//using AJudge.Application.DTO.ProblemsDTO;
//using AJudge.Domain.Entities;
//using AJudge.Domain.RepoContracts;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Metadata.Ecma335;
//using System.Text;
//using System.Threading.Tasks;

//namespace AJudge.Application.services
//{
//    public class ProblemService : IProblemService
//    {
//        private IUnitOfWork _unitOfWork;
//        public ProblemService(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork=unitOfWork;
//        }

//        public Task<bool> AddProblem(ProblemDTO problemDTO)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<bool> ChangeProblemStatement(ProblemDTO problemDTO)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<bool> DeleteProblem(int problemId)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<List<ProblemDTO>> GetAllProblems(string sortBy,bool isAsinding=true,int pageNumber=1,int pageSize=100)
//        {
//          List<Problem> problems=await _unitOfWork.Problem.GetAllInPage(sortBy, isAsinding, pageNumber, pageSize);

//            // List<ProblemDTO>problemsDTO=problems.Select(x=>ProblemDTO.ConvertToProblemDTO(x)).ToList();

//            return problems?.Select(x=>ProblemDTO.ConvertToProblemDTO(x)).ToList() ?? new List<ProblemDTO>();
//        }

//        public Task<ProblemDTO> GetProblem(int problemId)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<List<ProblemDTO>> GetProblemsByDifficulty(string difficulty)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<List<ProblemDTO>> GetProblemsBySource(string source)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<List<ProblemDTO>> GetProblemsByTag(string tag)
//        {
//            throw new NotImplementedException();
//        }



//        public async Task<ProblemDetailsDTO?> GetProblemByName(string name)
//        {
//            Problem?problem = await _unitOfWork.Problem.GetSpecific(x => x.ProblemName == name, new[] { "Tags", "Submissions","InputOutputTestCases" });
//            if (problem == null)
//                return null;

//            string state = problem.Submissions
//           .OrderByDescending(x => x.SubmittedAt)
//           .Select(x => x.Result)
//           .FirstOrDefault() ?? "Unattempted";
//            List<string>? tagsName = problem.Tags.Select(x => x.Name).ToList();

//            var testCases = new InputOutputTestCasesDTO
//            {
//                InputTetCase = problem.InputOutputTestCases.Select(tc => tc.Input).ToList(),
//                OutPutTestCases = problem.InputOutputTestCases.Select(tc => tc.Output).ToList()
//            };

//            ProblemDetailsDTO problemDEtailsDTO = ProblemDetailsDTO.ConvertToProblemDetalsDTO(problem, state, tagsName, testCases);
//            return problemDEtailsDTO;
//        } 
//    }
//}

using AJudge.Application.DtO.ProblemsDTO;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Application.services;
using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public class ProblemService : IProblemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProblemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddProblem(ProblemDTO problemDTO)
        {
            var problem = new Problem
            {
                ProblemName = problemDTO.ProblemName,
                Description = problemDTO.Description ?? "No description provided",
                InputFormat = problemDTO.InputFormat ?? "No input format provided",
                OutputFormat = problemDTO.OutputFormat ?? "No output format provided",
                Rating = problemDTO.Rating,
                ProblemLink = problemDTO.ProblemLink ?? "No link provided",
                ProblemSource = problemDTO.ProblemSource ?? "Unknown source",
                ProblemSourceID = problemDTO.ProblemSourceID ?? "Unknown source ID",
                numberOfTestCases = problemDTO.numberOfTestCases > 0 ? problemDTO.numberOfTestCases : 1,
                ContestId = 5 // تأكد إن ContestId = 5 موجود في جدول Contests
            };


            await _unitOfWork.Problem.AddAsync(problem);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving problem to database: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<bool> ChangeProblemStatement(ProblemDTO problemDTO)
        {
            var problem = await _unitOfWork.Problem.GetByIdAsync(problemDTO.ProblemId);
            if (problem == null)
            {
                return false;
            }

            problem.ProblemName = problemDTO.ProblemName;
            problem.Description = problemDTO.Description;
            problem.InputFormat = problemDTO.InputFormat;
            problem.OutputFormat = problemDTO.OutputFormat;
            problem.Rating = problemDTO.Rating;
            problem.ProblemLink = problemDTO.ProblemLink;
            problem.ProblemSource = problemDTO.ProblemSource;
            problem.ProblemSourceID = problemDTO.ProblemSourceID;
            problem.numberOfTestCases = problemDTO.numberOfTestCases;

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteProblem(int problemId)
        {
            var problem = await _unitOfWork.Problem.GetByIdAsync(problemId);
            if (problem == null)
            {
                return false;
            }

            await _unitOfWork.Problem.DeleteAsync(problem);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<ProblemDTO> GetProblem(int problemId)
        {
            var problem = await _unitOfWork.Problem.GetByIdAsync(problemId);
            if (problem == null)
            {
                return null;
            }

            return ProblemDTO.ConvertToProblemDTO(problem);
        }

        public async Task<ProblemDetailsDTO?> GetProblemByName(string name)
        {
            var problem = await _unitOfWork.Problem.GetSpecific(x => x.ProblemName == name, new[] { "Tags", "Submissions", "InputOutputTestCases" });
            if (problem == null)
            {
                return null;
            }

            string state = problem.Submissions
                .OrderByDescending(x => x.SubmittedAt)
                .Select(x => x.Result)
                .FirstOrDefault() ?? "Unattempted";
            List<string>? tagsName = problem.Tags?.Select(x => x.Name).ToList();

            var testCases = new InputOutputTestCasesDTO
            {
                InputTetCase = problem.InputOutputTestCases?.Select(tc => tc.Input).ToList() ?? new List<string>(),
                OutPutTestCases = problem.InputOutputTestCases?.Select(tc => tc.Output).ToList() ?? new List<string>()
            };

            return ProblemDetailsDTO.ConvertToProblemDetailsDTO(problem, state, tagsName, testCases);
        }

        public async Task<List<ProblemDTO>> GetAllProblems(string sortBy, bool isAsinding = true, int pageNumber = 1, int pageSize = 100)
        {
            var problems = await _unitOfWork.Problem.GetAllInPage(sortBy, isAsinding, pageNumber, pageSize);
            return problems?.Select(ProblemDTO.ConvertToProblemDTO).ToList() ?? new List<ProblemDTO>();
        }

        public async Task<List<ProblemDTO>> GetProblemsByTag(string tag)
        {
            var problems = await _unitOfWork.Problem.GetSpecificList(x => x.Tags.Any(t => t.Name == tag), new[] { "Tags" });
            return problems?.Select(ProblemDTO.ConvertToProblemDTO).ToList() ?? new List<ProblemDTO>();
        }

        public async Task<List<ProblemDTO>> GetProblemsByDifficulty(string difficulty)
        {
            var problems = await _unitOfWork.Problem.GetSpecificList(x => x.Rating.ToString() == difficulty);
            return problems?.Select(ProblemDTO.ConvertToProblemDTO).ToList() ?? new List<ProblemDTO>();
        }

        public async Task<List<ProblemDTO>> GetProblemsBySource(string source)
        {
            var problems = await _unitOfWork.Problem.GetSpecificList(x => x.ProblemSource == source);
            return problems?.Select(ProblemDTO.ConvertToProblemDTO).ToList() ?? new List<ProblemDTO>();
        }
    }
}