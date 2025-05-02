using AJudge.Domain.Entities;

namespace AJudge.Application.DtO.ProblemsDTO
{
    public class ProblemDetailsDTO
    {
        public string ProblemName { get; set; }
        public string ProblemDescription { get; set; }
        public int ProblemRating { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public string ProblemSource { get; set; }
        public string SubmissionState { get; set; }
        public List<string> TagsName { get; set; }
        public InputOutputTestCasesDTO InputOutputTestCases { get; set; }




        public static ProblemDetailsDTO ConvertToProblemDetalsDTO(Problem problem, string submissionState, List<string> tagsNames, InputOutputTestCasesDTO inputOutputTestCases)
        {
            return new ProblemDetailsDTO
            {
                ProblemName = problem.ProblemName,
                ProblemDescription = problem.Description,
                ProblemRating = problem.Rating,
                InputFormat = problem.InputFormat,
                OutputFormat = problem.OutputFormat,
                ProblemSource = problem.ProblemSource,
                SubmissionState = submissionState,
                TagsName = tagsNames,
                InputOutputTestCases = inputOutputTestCases
            };



        }
    }
}