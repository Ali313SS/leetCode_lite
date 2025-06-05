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

        /// <summary>
        /// Sends an email message  to the specified recipient.
        /// </summary>
        /// <param name="message">The body content of the email message. Supports HTML format.</param>
        /// <param name="recipient">The recipient's email address.</param>
        /// <param name="subject">The subject of the email. Defaults to "Test Email" if not specified.</param>
        /// <returns>A task that represents the asynchronous send operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the email fails to send.</exception>
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
