using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO.UserDTOS
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
            };
        }
    }
}
