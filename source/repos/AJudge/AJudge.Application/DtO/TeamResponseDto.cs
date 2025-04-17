using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.DtO
{
    public class TeamResponseDto
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TeamMemberDto> Members { get; set; } = new();
    }
}
