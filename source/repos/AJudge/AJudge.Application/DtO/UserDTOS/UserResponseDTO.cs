using AJudge.Domain.Entities;


namespace AJudge.Application.DTO.UserDTOS
{
    /// <summary>
    /// Data Transfer Object for returning basic user information.
    /// </summary>
    public class UserResponseDTO
    {
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's birth date, if available.
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the URL or path to the user's profile picture.
        /// </summary>
        public string? ProfilePicture { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user was last seen online.
        /// </summary>
        public DateTime? LastSeen { get; set; }

        /// <summary>
        /// Gets or sets the total count of problems the user has attempted.
        /// </summary>
        public int ProblemsTriedCount { get; set; }

        /// <summary>
        /// Gets or sets the user's affiliated club or university.
        /// </summary>
        public string? ClubUniversity { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user registered.
        /// </summary>
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
