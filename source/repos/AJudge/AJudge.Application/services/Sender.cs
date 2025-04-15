using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
namespace AJudge.Application.services
{
    public class Sender: ISender
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;
        public Sender(string smtpServer, int port, string fromAddress, string password)
        {
            _smtpClient = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(fromAddress, password),
                EnableSsl = true
            };
            _fromAddress = fromAddress;
        }
        public void SendMessage(string message, string recipient)
        {
            var mailMessage = new MailMessage(_fromAddress, recipient)
            {
                Subject = "Test Email",
                Body = message,
                IsBodyHtml = true
            };
            _smtpClient.Send(mailMessage);
        }
    }
 
}
