using AJudge.Domain.Entities;

namespace AJudge.Application.DtO.ProblemsDTO
{
    public class ProblemDetailsDTO
    {

       


        /// <summary>
        /// The name/title of the problem.
        /// </summary>
        public string ProblemName { get; set; }

        /// <summary>
        /// The ID of the problem in the source platform.
        /// </summary>
        public int SourceID { get; set; }

        /// <summary>
        /// The detailed description of the problem.
        /// </summary>
        public string ProblemDescription { get; set; }

        /// <summary>
        /// The difficulty rating of the problem.
        /// </summary>
        public int ProblemRating { get; set; }

        /// <summary>
        /// Description of the input format for the problem.
        /// </summary>
        public string InputFormat { get; set; }

        /// <summary>
        /// Description of the output format for the problem.
        /// </summary>
        public string OutputFormat { get; set; }

        /// <summary>
        /// The source platform of the problem (e.g., Codeforces).
        /// </summary>
        public string ProblemSource { get; set; }

        /// <summary>
        /// The submission state of the problem for the current user (e.g., solved, attempted).
        /// </summary>
        public string SubmissionState { get; set; }

        /// <summary>
        /// List of tags associated with the problem.
        /// </summary>
        public List<string> TagsName { get; set; }

        /// <summary>
        /// List of input/output test cases for the problem.
        /// </summary>
        public List<InputOutputTestCasesDTO> TestCases { get; set; }




        public static ProblemDetailsDTO ConvertToProblemDetalsDTO(Problem problem, string submissionState, List<string> tagsNames, List<InputOutputTestCasesDTO> testCases)
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
                TestCases = testCases
            };



        }
    }
}