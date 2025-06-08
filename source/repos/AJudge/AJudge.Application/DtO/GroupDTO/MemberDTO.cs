using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DTO.GroupDTO
{
    /// <summary>
    /// Data transfer object representing a member of a group, including username and role
    /// </summary>
    public class MemberDTO
    {
        /// <summary>
        /// Gets or sets the username of the group member.
        /// This property is optional.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the role of the member within the group (e.g., Leader, Member).
        /// </summary>
        public string Role { get; set; }

    }
}
