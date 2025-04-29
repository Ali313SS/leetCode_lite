using AJudge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DTO.GroupDTO
{
   public class GroupDTO
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public int GroupLeader { get; set; }
        public PrivacyType Privacy { get; set; }
        public String? ProfilePicture { get; set; }
        public string? Description { get; set; }
    }
}
