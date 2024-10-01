using System.Net.Mail;
using System.Net;

namespace NuochoaHuxtah.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("nhtthanh633@gmail.com", "uilasdvtevcgridx")
            };

            return client.SendMailAsync(
                new MailMessage(from: "nhtthanh633@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
