using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class ChatBot
    {
        public int BotId { get; set; }
        public int UserId { get; set; }

        // Navigation Properties
        public User User { get; set; }
    }
}
