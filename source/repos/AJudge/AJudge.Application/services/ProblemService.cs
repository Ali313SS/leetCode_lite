using AJudge.Application.DtO.ProblemsDTO;
using AJudge.Application.DTO.ProblemsDTO;
using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public class ProblemService : IProblemService
    {
        private IUnitOfWork _unitOfWork;
        public ProblemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public Task<bool> AddProblem(ProblemDTO problemDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ChangeProblemStatement(ProblemDTO problemDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProblem(int problemId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProblemDTO>> GetAllProblems(string sortBy,bool isAsinding=true,int pageNumber=1,int pageSize=100)
        {
          List<Problem> problems=await _unitOfWork.Problem.GetAllInPage(sortBy, isAsinding, pageNumber, pageSize);

            // List<ProblemDTO>problemsDTO=problems.Select(x=>ProblemDTO.ConvertToProblemDTO(x)).ToList();

            return problems?.Select(x=>ProblemDTO.ConvertToProblemDTO(x)).ToList() ?? new List<ProblemDTO>();
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



        public async Task<ProblemDetailsDTO?> GetProblemByName(string name)
        {
            Problem?problem = await _unitOfWork.Problem.GetSpecific(x => x.ProblemName == name, new[] { "Tags", "Submissions","InputOutputTestCases" });
            if (problem == null)
                return null;

            string state = problem.Submissions
           .OrderByDescending(x => x.SubmittedAt)
           .Select(x => x.Result)
           .FirstOrDefault() ?? "Unattempted";
            List<string>? tagsName = problem.Tags.Select(x => x.Name).ToList();

            var testCases = new InputOutputTestCasesDTO
            {
//                InputTetCase = problem.InputOutputTestCases.Select(tc => tc.Input).ToList(),
  //              OutPutTestCases = problem.InputOutputTestCases.Select(tc => tc.Output).ToList()
            };

            ProblemDetailsDTO problemDEtailsDTO = ProblemDetailsDTO.ConvertToProblemDetalsDTO(problem, state, tagsName, testCases);
            return problemDEtailsDTO;
        } 
    }
}
