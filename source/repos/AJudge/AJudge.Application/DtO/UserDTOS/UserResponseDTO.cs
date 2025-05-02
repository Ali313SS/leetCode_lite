using AJudge.Domain.Entities;


namespace AJudge.Application.DTO.UserDTOS
{
    public class UserResponseDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime? LastSeen { get; set; }
        public int ProblemsTriedCount { get; set; }
        public string? ClubUniversity { get; set; }
        public DateTime RegisterAt { get; set; }

        public static UserResponseDTO ConvertToUserResponse(User user)
        {
            return new UserResponseDTO
            {
                Username = user.Username,
                Email = user.Email,
                BirthDate = user.BirthDate,
                ProfilePicture = user.ProfilePicture,
                LastSeen = user.LastSeen,
                ProblemsTriedCount = user.ProblemsTriedCount,
                ClubUniversity = user.ClubUniversity,
                RegisterAt=user.RegisterAt
            };
        }
    }
}
