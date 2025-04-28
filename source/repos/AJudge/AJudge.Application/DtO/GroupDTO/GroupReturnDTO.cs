using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DTO.GroupDTO
{
 
    public class CContest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public DateTime Created { get; set; }
        public DateTime Ended { get; set; }

    }
    public  class GroupReturnDTO
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public PrivacyType Privacy { get; set; }
        public string? Description { get; set; }
        public String? ProfilePicture { get; set; }
        public DateTime Created { get; set; }
        // Navigation Properties
        public ICollection<CContest> Contests { get; set; } = new List<CContest>();

    }
}