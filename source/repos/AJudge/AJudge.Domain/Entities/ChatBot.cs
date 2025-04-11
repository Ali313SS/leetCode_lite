using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Domain.Entities
{
    public class ChatBot
    {
        [Key]
        public int BotId { get; set; }
        public int UserId { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
