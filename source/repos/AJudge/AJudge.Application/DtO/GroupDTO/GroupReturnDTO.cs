using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DTO.GroupDTO
{
    // <summary>
    /// Represents  contest entity
    /// </summary>
    public class CContest
    {
      
        /// <summary>
        /// Gets or sets the unique identifier of the contest.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the contest.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the contest was created or started.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the contest ended.
        /// </summary>
        public DateTime Ended { get; set; }

    }
    /// <summary>
    /// Data transfer object for returning group details, including associated contests.
    /// </summary>
    public class GroupReturnDTO
    {
       
        //}
        /// <summary>
        /// Gets or sets the unique identifier of the group.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the privacy setting of the group (e.g., Public or Private).
        /// </summary>
        public PrivacyType Privacy { get; set; }

        /// <summary>
        /// Gets or sets the group description. This field is optional.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the profile picture for the group (URL or base64 string). This field is optional.
        /// </summary>
        public string? ProfilePicture { get; set; }

        /// <summary>
        /// Gets or sets the date and time the group was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the collection of contests associated with the group.
        /// </summary>
        public ICollection<CContest> Contests { get; set; } = new List<CContest>();
    }
}