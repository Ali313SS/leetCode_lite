using AJudge.Domain.Entities;


namespace AJudge.Application.DtO.UserDTOS
{
    public class UserResponse_RelatedInfoDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime? LastSeen { get; set; }
        public int ProblemsTriedCount { get; set; }
        public string? ClubUniversity { get; set; }

        public List<string>? GroupsName { get; set; } 
        public List<string>? GroupsWhichYouCoached { get; set; } 
        public List<string>? YourCoaches { get; set; } 
        public List<string>? MyTrainers { get; set; } 
        public List<string>? Friends { get; set; } 



        public static UserResponse_RelatedInfoDTO ConvertToUser_RelatedInfoResponse(User user, List<string>groupName , List<string> yourCoaches, List<string> trainers,
            List<string> friends, List<string> groupsWhichYouCoached=null)
        {
            return new UserResponse_RelatedInfoDTO
            {
                Username = user.Username,
                Email = user.Email,
                BirthDate = user.BirthDate,
                ProfilePicture = user.ProfilePicture,
                LastSeen = user.LastSeen,
                ProblemsTriedCount = user.ProblemsTriedCount,
                ClubUniversity = user.ClubUniversity,
                GroupsName = groupName,
                GroupsWhichYouCoached=groupsWhichYouCoached,
                YourCoaches = yourCoaches,
                MyTrainers = trainers   ,
                Friends=friends
            };
        }
    }
}
