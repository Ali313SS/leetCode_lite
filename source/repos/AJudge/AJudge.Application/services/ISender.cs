using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJudge.Application.services
{
    public interface ISender
    {
        void SendMessage(string message, string recipient);
    }
    
      
}
