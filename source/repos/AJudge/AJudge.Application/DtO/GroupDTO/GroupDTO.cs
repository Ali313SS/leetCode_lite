using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DTO.GroupDTO
{
    /// <summary>
    /// Data transfer object representing a user group.
    /// </summary>
    public class GroupDTO
    {
       
        /// <summary>
        /// Gets or sets the unique identifier of the group.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user ID of the group leader (admin).
        /// </summary>
        public int GroupLeader { get; set; }

        /// <summary>
        /// Gets or sets the privacy setting of the group (e.g., Public or Private).
        /// </summary>
        public PrivacyType Privacy { get; set; }

        /// <summary>
        /// Gets or sets the URL or base64 string of the group’s profile picture.
        /// This property is optional.
        /// </summary>
        public string? ProfilePicture { get; set; }

        /// <summary>
        /// Gets or sets the group's description.
        /// This property is optional.
        /// </summary>
        public string? Description { get; set; }
    }
}
