using AJudge.Application.DTO.ProblemsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public interface IProblemService
    {
        Task<bool> AddProblem(ProblemDTO problemDTO);
        Task<bool> ChangeProblemStatement(ProblemDTO problemDTO);
        Task<bool> DeleteProblem(int problemId);
        Task<ProblemDTO> GetProblem(int problemId);
        Task<List<ProblemDTO>> GetAllProblems();
        Task<List<ProblemDTO>> GetProblemsByTag(string tag);
        Task<List<ProblemDTO>> GetProblemsByDifficulty(string difficulty);
        Task<List<ProblemDTO>> GetProblemsBySource(string source);
    }
}
